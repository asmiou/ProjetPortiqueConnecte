#Import library
import sys

import numpy as np
import pandas as pd


#from sklearn.preprocessing import StandardScaler
from sklearn import preprocessing
#import pickle
import joblib as jl

#parameters
assetFolder=sys.argv[1]
token=sys.argv[2]
executed=False

#Import Data
#n=['TimesTamp','ECP', 'Antenna', 'RSSI']

n=['TimesTamp', 'ECP', 'Antenna', 'RSSI', 'Channel', 'Adress']
p=assetFolder+'/1-RawData/'+token+'.csv'
print(p)
d=','

def importData(path, delimit,cols):
    return pd.read_csv(path, sep=delimit,names=cols)

data=importData(p,d,n)

print("imported...")
# TYPAGE DES CHAMPS
def typage(data):
    data['ECP']=data['ECP'].astype(str)
    data['TimesTamp']=data['TimesTamp'].astype('int64')
    data['RSSI']=data['RSSI'].astype('float64')
    data['Antenna']=data['Antenna'].astype('int64')
    return data

data=typage(data)

print("typed...")

#Build DataSet
from scipy.stats import variation 

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

dataSet=generateDataSet(data,'ECP')

print("build dataset...")

#Export Data
def exportData(data, path):
    data.to_csv(path, index = None, header=False)

#Export allinOne
#exportPath = assetFolder+'/2-DataSet/'+token+'.csv'
#exportData(dataSet, exportPath)

#print("exported...")
#executed=True

#######################################################################################
def scaleData(data):
    scaler = StandardScaler()
    #scaler.fit(data)
    return scaler.fit_transform(data)

#X_toPredict = scaleData(dataSet.loc[:,'RC':'A4'])
X_toPredict = preprocessing.normalize(dataSet.loc[:,'RC':'A4'])
print("Normalized...")

knnFilename = assetFolder +'/knn_Model2.sav'
print('path :',knnFilename)


#knnModel = pickle.load(open(knnFilename, 'rb'))
knnModel = jl.load(knnFilename)

print("load models...")

knn_pred = knnModel.predict(X_toPredict)

print("prediction...")

dataSet['FP_KNN']=knn_pred

print("Done...")
print(dataSet)

exportPath = assetFolder+'/3-PredictedData/'+token+'.csv'
exportData(dataSet, exportPath)

print("exported Done....")


