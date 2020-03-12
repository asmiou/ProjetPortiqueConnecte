from flask import Flask, jsonify, make_response
from flask import request as req
import requests
import pandas as pd
from scipy.stats import variation 
import pickle
from sklearn.preprocessing import StandardScaler

app = Flask(__name__)

cols=['ECP','RSSI','TimesTamp', 'Antenna']
delimitor=';'


#cols=['TimesTamp', 'ECP', 'Antenna', 'RSSI', 'Channel', 'Adress']
#delimitor=','
TOKEN=""
PATH=""
data = ""
dataSet=""
X_toPredict =""
knnFilename = './knn_Model.sav'

#Import des données
def importData(path, delimit,cols):
    return pd.read_csv(path, sep=delimit,names=cols)

# TYPAGE DES CHAMPS
def typage(data):
    #data['ECP']=data['ECP'].astype(str)
    data['TimesTamp']=data['TimesTamp'].astype('int64')
    data['RSSI']=data['RSSI'].astype('float64')
    #data['Antenna']=data['Antenna'].astype('int64')
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
    #scaler.fit(data)
    return scaler.fit_transform(data)


#Export Data
def exportData(data, path):
    data.to_csv(path, index = None, header=False)

@app.route("/flaskapp/predict")
def predictFromUrl():
    TOKEN = req.args.get('token')
    PATH="./1-RawData/"+TOKEN+".csv"
    print(PATH)
   # try:
    #Chargement des données
    data=importData(PATH,delimitor,cols)
    print("imported...", len(data))
    print(data.head(5))

    #typage des données
    data=typage(data)
    print("typed...")

    #Regroupement par ECP
    dataSet=generateDataSet(data,'ECP')
    print("regrouped..." , len(dataSet))

    #Mise en echelle
    X_toPredict = scaleData(dataSet.loc[:,'RC':'A4'])
    print("xpredict...")

    #LoadModel
    knnModel = pickle.load(open(knnFilename, 'rb'))
    print("model loaded...")
    #Prediction knn
    knn_pred = knnModel.predict(X_toPredict)
    print("predicted...")

    #Rajout de la dataSet
    dataSet['FP_KNN']=knn_pred
    print("dataset predict...", dataSet.shape)

    #Export des données
    exportPath = './3-PredictedData/'+TOKEN+'.csv'
    exportData(dataSet, exportPath)
    print("exported...")

    print("Done...")
    return make_response(jsonify({'message':'Prediction successfull', 'code':200}),200)
   # except:
   #     print("Error found...")
    #    return make_response(jsonify({'message':'Error script python', 'code':500}),500) '''


app.run(debug=True)
