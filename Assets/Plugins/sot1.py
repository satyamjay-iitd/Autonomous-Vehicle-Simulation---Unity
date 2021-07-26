from ctypes import *
so_file="./sem.so"
import mmap
import struct
import numpy as np
import cv2
import base64
from PIL import Image
from io import BytesIO
from matplotlib import pyplot as plt
from voxels import processTopView
sem=CDLL(so_file)
sem.readMMF.restype=c_char_p
mysem=sem.semaphore_open(bytes("lockForMMF", encoding='utf-8'), sem.getO_Creat(), 1)
sem.post(mysem)
#print(mysem)
#vb=ba1=bytearray(struct.pack("i", 123))
#sem.wait(mysem)
#print("here1")
#sem.post(mysem)
shm_fd=sem.shared_mem_open(bytes("imageTransfer", encoding='utf-8'), sem.getO_CREAT_ORDWR())
mmf=sem.mmap_obj(1000000, shm_fd)
#print(sem.readMMF(mmf, 20))
while True:
  sem.wait(mysem)
  img_str = sem.readMMF(mmf, 1000000)
  sem.post(mysem)
  if img_str=='':
    print("emopty")
  #else:
  #  print(img_str)
  #try:
  image = Image.open(BytesIO(base64.b64decode(img_str)))
  image = np.asarray(image)
  cv2.imshow('res', image)
  cv2.waitKey(1)
  #processTopView(image)
    #image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
    #cv2.namedWindow('points', cv2.WINDOW_NORMAL)
    #cv2.imshow('points', image)
    #cv2.waitKey(100)
  #except:
   # print("Eception occured")
