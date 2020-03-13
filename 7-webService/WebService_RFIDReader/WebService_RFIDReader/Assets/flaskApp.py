from flask import Flask, jsonify, make_response
from flask import request as req
import requests
import pandas as pd
from scipy.stats import variation 
import pickle
from sklearn.preprocessing import StandardScaler

app = Flask(__name__)

cols=['ECP','RSSI','TimesTamp', 'Antenna']
#cols=['TimesTamp', 'ECP', 'Antenna', 'RSSI', 'Channel', 'Adress']

delimitor=','
TOKEN=""
PATH=""
data = ""
dataSet=""
X_toPredict =""

knnFilename = './knn_Model.sav'
regLogFilename = './logreg_Model.sav'
svmFilename = './gaussianSVM_Model.sav'

#Import des données
def importData(path, delimit,cols):
    return pd.read_csv(path, sep=delimit,names=cols)

# TYPAGE DES CHAMPS
def typage(data):
    data['ECP']=data['ECP'].astype(str)
    data['TimesTamp']=data['TimesTamp'].astype('int64')
    data['RSSI']=data['RSSI'].astype('float64')
    data['Antenna']=data['Antenna'].astype('int64')
    return data

#Build dataSet
def generateDataSet(data, groupedBy):
    grouped_df = data.groupby(groupedBy) #On groupe les données par ..."Etiquetes"
    
    dataSet = pd.DataFrame({}) #Création d'une nouvelle dataFrame
    
    #Création des variables intermédiaire pour la récupération des champs
    ecp=pd.Series(grouped_df.ECP.unique().index,name="ECP")
    rc= pd.Series(grouped_df.ECP.count(),name="readcount")
    minRSSI= pd.Series(grouped_df.RSSI.min(),name="minRssi")
    maxRSSI= pd.Series(grouped_df.RSSI.max(),name="maxRssi")
    mean=pd.Series(grouped_df.RSSI.mean(),name="meanRssi")
    startTime=pd.Series(grouped_df.TimesTamp.min(),name="startTime")
    endTime=pd.Series(grouped_df.TimesTamp.max(),name="endTime")
    #diff=pd.Series(grouped_df.RSSI.diff,name="difference")
    
    #Mise à jour des champs de la dataFrame
    dataSet['ECP']=ecp.values
    dataSet['RC']=rc.values
    dataSet['MIN']=minRSSI.values
    dataSet['MAX']=maxRSSI.values
    dataSet['MEAN']=mean.values
    dataSet['START']=startTime.values
    dataSet['END']=endTime.values
    dataSet['DURATION']=endTime.values-startTime.values
    cv={}
    a1={}
    a2={}
    a3={}
    a4={}
    for key, item in grouped_df:
        cv[key]=variation(grouped_df['RSSI'].get_group(key).values,axis=0)
        a1[key]=0
        a2[key]=0
        a3[key]=0
        a4[key]=0
        for a in grouped_df['Antenna'].get_group(key).values:
            if(a==1): a1[key]+=1
            if(a==2): a2[key]+=1
            if(a==3): a3[key]+=1
            if(a==4): a4[key]+=1
                
    dataSet['CV']=pd.Series(cv).values
    dataSet['A1']=pd.Series(a1).values
    dataSet['A2']=pd.Series(a2).values
    dataSet['A3']=pd.Series(a3).values
    dataSet['A4']=pd.Series(a4).values
    
    return dataSet

#Mise en echelle des données
def scaleData(data):
    scaler = StandardScaler()
    return scaler.fit_transform(data)


#Export Data
def exportData(data, path):
    data.to_csv(path, index = None, header=False)

#System de vote entre les 3 models de prediction
def voteSystem(data):
    verdict=[]
    for d in data.values:
        cote= d[-3]+d[-2]+d[-1]
        
        if(cote>=2):
            verdict.append(1)  
        else:
            verdict.append(0)
    return verdict


@app.route("/")
def index():
    try:
        return make_response(jsonify({'message':'Server is run...', 'code':200}),200)
    except:
        return make_response(jsonify({'message':'Server is off...', 'code':500}),500)

@app.route("/flaskapp/predict")
def predictFromUrl():
    TOKEN = req.args.get('token')

    if TOKEN is None:
        return make_response(jsonify({'message':'Error le token ne doit pas être vide', 'code':404}),404)

    PATH="./1-RawData/"+TOKEN+".csv"

    try:
        #Chargement des données
        data=importData(PATH,delimitor,cols)
        
        #remove naValues
        data=data.dropna()

        #typage des données
        data=typage(data)

        #Regroupement par ECP
        dataSet=generateDataSet(data,'ECP')

        #Mise en echelle
        X_toPredict = scaleData(dataSet.loc[:,'RC':'A4'])

        #-----------Knn
        #LoadModel
        knnModel = pickle.load(open(knnFilename, 'rb'))
        #Prediction knn
        knn_pred = knnModel.predict(X_toPredict)

        #-----------Regression logistique
        #LoadModel
        regLogModel = pickle.load(open(regLogFilename, 'rb'))
        #Prediction RegLog
        regLog_pred = regLogModel.predict(X_toPredict)

        #-----------SVM
        #LoadModel
        svmModel = pickle.load(open(svmFilename, 'rb'))
        #Prediction knn
        svm_pred = svmModel.predict(X_toPredict)

        #Rajout de la dataSet
        dataSet['FP_RL']=regLog_pred
        dataSet['FP_SVM']=svm_pred
        dataSet['FP_KNN']=knn_pred

        #Systeme de vote
        verdict=voteSystem(dataSet)
        dataSet['VERDICT']=verdict

        #Export des données
        #exportPath = './3-PredictedData/'+TOKEN+'.csv'
        #exportData(dataSet, exportPath)

        classified = {'ECP':list(dataSet['ECP']), 'FP':list(dataSet['VERDICT'])}

        msg={'message':'Prediction successfull', 
        'code':200,
        'data':{'token':TOKEN, 'lenght':len(dataSet), 'classified':classified}
        }

        return make_response(jsonify(msg),200)
    except:
        return make_response(jsonify({'message':'Error script python', 'code':500}),500)

@app.route("/flaskapp/check-connect")
def checkConnexion():
    try:

        return make_response(jsonify({'message':'Success server on', 'code':200}),200)
    except:
        return make_response(jsonify({'message':'Error server off', 'code':500}),500)


#app.run(debug=True)
app.run()
