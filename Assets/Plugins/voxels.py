import numpy as np
from matplotlib import pyplot as plt
import cv2
import time
from skimage import morphology
count=2
#print(np.repeat((points[:, 2]>0.2)[:, np.newaxis], 4, axis=1))
#points=points[points[:, 2]>0.2, :]
#from mpl_toolkits.mplot3d import Axes3D
#import random
#fig = plt.figure()
#ax = Axes3D(fig)
#ax.set_xlim3d(-20, 20)
#ax.set_ylim3d(-20, 20)
#ax.set_zlim3d(-20, 20)
#ax.scatter(points[:, 0], points[:, 1], points[:, 2])
#plt.show()
#print(points)
#points=(points/0.5).astype(int)
#voxel_grid=np.zeros((80, 80, 20), dtype='bool')
#for cord in points:
#	voxel_grid[cord[0]+40, cord[1]+40, cord[2]]=True
#img=np.sum(voxel_grid, axis=2)
#print(img)
#img=img>0
#selem=np.ones((5, 5), dtype='bool')
#img=morphology.binary_opening(img, selem)
#plt.imshow(img)
#plt.show()
#selem=np.ones((5, 5), dtype='bool')
#img=morphology.binary_closing(img, selem)
#plt.imshow(img)
#plt.show()
#img2=np.zeros(img.shape, dtype='int32')
#img2[img]=1
#plt.imshow(img)
#plt.show()
#yn, xn=np.nonzero(img2)
#minarrx=[]
#minarry=[]
#maxarrx=[]
#maxarry=[]
#for i in range(xn.shape[0]):
#  if img2[yn[i], xn[i]]==1:
#   qu=[(xn[i], yn[i])]
#    img2[yn[i], xn[i]]=count
#    minarrx.append(xn[i])
#    minarry.append(yn[i])
#    maxarrx.append(xn[i])
#    maxarry.append(yn[i])
#    ind=count-2
#    while len(qu)>0:
#      curr=qu.pop(0)
#      p=curr[0]+1
#      q=curr[1]+1
#      if p<img2.shape[1] and q<img2.shape[0] and img2[q, p]==1:
#        img2[q, p]=count
#        qu.append((p, q))
#        maxarrx[ind]=max(p, maxarrx[ind])
#        maxarry[ind]=max(q, maxarry[ind])
#        minarrx[ind]=min(p, minarrx[ind])
#        minarry[ind]=min(q, minarry[ind])
#      p=curr[0]-1
#      q=curr[1]-1
#      if p>=0 and q>=0 and img2[q, p]==1:
#        img2[q, p]=count
#        qu.append((p, q))
#        maxarrx[ind]=max(p, maxarrx[ind])
#        maxarry[ind]=max(q, maxarry[ind])
#        minarrx[ind]=min(p, minarrx[ind])
#        minarry[ind]=min(q, minarry[ind])     
#      q=curr[1]-1
#      if q>=0 and img2[q, curr[0]]==1:
#        img2[q, curr[0]]=count
#        qu.append((curr[0], q))
#        maxarrx[ind]=max(curr[0], maxarrx[ind])
#        maxarry[ind]=max(q, maxarry[ind])
#        minarrx[ind]=min(curr[0], minarrx[ind])
def processTopView(img, selem=np.ones((3, 3), dtype='bool'), res=0.5):
  count=2
  img=img==255
  img=img[:, :, 0]
  img=morphology.binary_closing(img, selem)
  img2=np.zeros(img.shape, dtype='int32')
  img2[img]=1
  yn, xn=np.nonzero(img2)
  minarrx=[]
  minarry=[]
  maxarrx=[]
  maxarry=[]
  for i in range(xn.shape[0]):
    if img2[yn[i], xn[i]]==1:
      qu=[(xn[i], yn[i])]
      img2[yn[i], xn[i]]=count
      minarrx.append(xn[i])
      minarry.append(yn[i])
      maxarrx.append(xn[i])
      maxarry.append(yn[i])
      ind=count-2
      while len(qu)>0:
        curr=qu.pop(0)
        p=curr[0]+1
        q=curr[1]+1
        if p<img2.shape[1] and q<img2.shape[0] and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        q=curr[1]-1
        if p>=0 and q>=0 and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])     
        q=curr[1]-1
        if q>=0 and img2[q, curr[0]]==1:
          img2[q, curr[0]]=count
          qu.append((curr[0], q))
          maxarrx[ind]=max(curr[0], maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(curr[0], minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        if p>=0 and img2[curr[1], p]==1:
          img2[curr[1], p]=count
          qu.append((p, curr[1]))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(curr[1], maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(curr[1], minarry[ind])
        p=curr[0]+1
        q=curr[1]-1
        if p<img.shape[1] and q>=0 and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        q=curr[1]+1
        if p>=0 and q<img.shape[0] and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]+1
        if p<img.shape[1] and img2[curr[1], p]==1:
          img2[curr[1], p]=count
          qu.append((p, curr[1]))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(curr[1], maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(curr[1], minarry[ind])
        q=curr[1]+1
        if q<img.shape[0] and img2[q, curr[0]]==1:
          img2[q, curr[0]]=count
          qu.append((curr[0], q))
          maxarrx[ind]=max(curr[0], maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(curr[0], minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
      count+=1
  i=count-3
  bboxes=[]
  while(i>=0):
    yp, xp=np.nonzero(img2[minarry[i]:maxarry[i]+1, minarrx[i]:maxarrx[i]+1])
    yp=yp+minarry[i]
    xp=xp+minarrx[i]
    cnt=np.concatenate((xp[:, np.newaxis], yp[:, np.newaxis]), axis=1)
    rect = cv2.minAreaRect(cnt)
    #print(rect)
    box = cv2.boxPoints(rect)
    bboxes.append(box)
    box = np.int0(box)
    img2=cv2.drawContours(img2,[box],0,(6,6,6),1)
    i-=1
  #print(img2.shape)
  plt.imshow(img2)
  plt.pause(0.001)
  #cv2.waitKey(100)
  bboxes=np.array(bboxes)-img2.shape[0]/2
  return bboxes*res
	
 
def findbb(points, res=0.5, floor=-2.5, grid_size=(80, 80, 20), selem=np.ones((3, 3), dtype='bool')):
  count=2
  points=points[points[:, 2]>floor, :]
  points=(points/res).astype(int)
  voxel_grid=np.zeros(grid_size, dtype='bool')
  offset_x=grid_size[0]//2
  offset_y=grid_size[1]//2
  for cord in points:
  	voxel_grid[cord[0]+offset_x, cord[1]+offset_y, cord[2]+3]=True
  img=np.sum(voxel_grid, axis=2)>0
  img=morphology.binary_closing(img, selem)
  img2=np.zeros(img.shape, dtype='int32')
  img2[img]=1
  yn, xn=np.nonzero(img2)
  minarrx=[]
  minarry=[]
  maxarrx=[]
  maxarry=[]
  for i in range(xn.shape[0]):
    if img2[yn[i], xn[i]]==1:
      qu=[(xn[i], yn[i])]
      img2[yn[i], xn[i]]=count
      minarrx.append(xn[i])
      minarry.append(yn[i])
      maxarrx.append(xn[i])
      maxarry.append(yn[i])
      ind=count-2
      while len(qu)>0:
        curr=qu.pop(0)
        p=curr[0]+1
        q=curr[1]+1
        if p<img2.shape[1] and q<img2.shape[0] and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        q=curr[1]-1
        if p>=0 and q>=0 and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])     
        q=curr[1]-1
        if q>=0 and img2[q, curr[0]]==1:
          img2[q, curr[0]]=count
          qu.append((curr[0], q))
          maxarrx[ind]=max(curr[0], maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(curr[0], minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        if p>=0 and img2[curr[1], p]==1:
          img2[curr[1], p]=count
          qu.append((p, curr[1]))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(curr[1], maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(curr[1], minarry[ind])
        p=curr[0]+1
        q=curr[1]-1
        if p<img.shape[1] and q>=0 and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]-1
        q=curr[1]+1
        if p>=0 and q<img.shape[0] and img2[q, p]==1:
          img2[q, p]=count
          qu.append((p, q))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
        p=curr[0]+1
        if p<img.shape[1] and img2[curr[1], p]==1:
          img2[curr[1], p]=count
          qu.append((p, curr[1]))
          maxarrx[ind]=max(p, maxarrx[ind])
          maxarry[ind]=max(curr[1], maxarry[ind])
          minarrx[ind]=min(p, minarrx[ind])
          minarry[ind]=min(curr[1], minarry[ind])
        q=curr[1]+1
        if q<img.shape[0] and img2[q, curr[0]]==1:
          img2[q, curr[0]]=count
          qu.append((curr[0], q))
          maxarrx[ind]=max(curr[0], maxarrx[ind])
          maxarry[ind]=max(q, maxarry[ind])
          minarrx[ind]=min(curr[0], minarrx[ind])
          minarry[ind]=min(q, minarry[ind])
      count+=1
  i=count-3
  bboxes=[]
  while(i>=0):
    yp, xp=np.nonzero(img2[minarry[i]:maxarry[i]+1, minarrx[i]:maxarrx[i]+1])
    yp=yp+minarry[i]
    xp=xp+minarrx[i]
    cnt=np.concatenate((xp[:, np.newaxis], yp[:, np.newaxis]), axis=1)
    rect = cv2.minAreaRect(cnt)
    #print(rect)
    box = cv2.boxPoints(rect)
    bboxes.append(box)
    #box = np.int0(box)
    #img2=cv2.drawContours(img2,[box],0,(6,6,6),1)
    i-=1
  #plt.imshow(img2)
  #plt.show()
  bboxes=np.array(bboxes)-img2.shape[0]/2
  return bboxes*res
  
#points=np.load('mydata_20.npy')
#t1=time.time()
#bb=findbb(points)
#print(time.time()-t1)
#print(bb.shape)
