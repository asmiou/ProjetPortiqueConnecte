import numpy as np
import pandas as pd
import sys

appData = sys.argv[1]
token = sys.argv[2]

path=appData+'/2-DataSet/'+token+'.csv'
print(path)

cols=["ECP","RC","MIN","MAX","MEAN","START","END","DURATION","CV","A1","A2","A3","A4"]
#cols=["ECP","RC","MIN","MAX","MEAN","START","END","DURATION","CV","A1","A2","A3","A4","FP"]
toPredict = pd.read_csv(path,names=cols, sep=',')
print("imported...")

from sklearn.preprocessing import StandardScaler

def scaleData(data):
    scaler = StandardScaler()
    scaler.fit(data)
    return scaler.transform(data)


X_toPredict = scaleData(toPredict.loc[:,'RC':'A4'])

print("scaled...")

import pickle

#logregFilename = appData +'/logreg_Model.sav'
#logregModel = pickle.load(open(logregFilename, 'rb'))

knnFilename = appData +'/knn_Model.sav'
knnModel = pickle.load(open(knnFilename, 'rb'))

#svmFilename = appData +'/gaussianSVC_Model.sav'
#svmModel = pickle.load(open(svmFilename, 'rb'))

print("load models...")
# Logistic Regression
#logreg_pred = logregModel.predict(X_toPredict)


#Knn Predict
knn_pred = knnModel.predict(X_toPredict)


#SVM Gaussien
#svm_pred = svmModel.predict(X_toPredict)

print("prediction...")

#toPredict['FP_LG']=logreg_pred
toPredict['FP_KNN']=knn_pred
#toPredict['FP_SVM']=svm_pred

def voteSystem(data):
    verdict=[]
    for d in data.values:
        cote= d[-3]+d[-2]+d[-1]
        
        if(cote>=2):
            verdict.append(1)  
        else:
            verdict.append(0)
    return verdict

#verdict=voteSystem(toPredict)
#toPredict['VERDICT']=verdict


print("Vote...")

#Export
pathExport=appData+'/3-PredictedData/'+token+'.csv'
toPredict.to_csv(pathExport, index = None, header=True)
print("Export predict" + pathExport)




