#include <fcntl.h>
#include <semaphore.h>
#include <stdio.h>
#include<stdlib.h>
#include<unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/shm.h>
#include <sys/stat.h>
#include <sys/mman.h>
sem_t* mysem[5];
void* ptr[5];
int count=0;
int count1=0;
int semaphore_open(char semname[], int oflag, int val){
	mysem[count]=sem_open(semname, oflag, 0644, val);
	count++;
	return count-1;
}
int getO_Creat(){
	return O_CREAT;
}

void wait(int ind){
	sem_wait(mysem[ind]);
}

void post(int ind){
	sem_post(mysem[ind]);
}

int getO_CREAT_ORDWR(){
	return O_CREAT | O_RDWR;
}

int shared_mem_open(char name[], int shm_flag){
	return  shm_open(name, shm_flag, 0666);
}

void ftrunc(int shm_fd, const int size){
	ftruncate(shm_fd, size);
}

int mmap_obj(int size, int shm_fd){
	ptr[count1]=mmap(0, size, PROT_WRITE | PROT_READ, MAP_SHARED, shm_fd, 0);
	count1++;
	return count1-1;
}

void writeMMF(char msg[], int mmap){
	sprintf(ptr[mmap], "%s", msg);
}

char* readMMF(int mmap, int size){
	char* str_read=(char*)malloc(size);
	memcpy(str_read, ptr[mmap], size);
	return str_read;
}
char* test(){
	char* gh=(char*)malloc(30*sizeof(char));
	strcpy(gh, "helloTest");
	return gh;
}
int main(){
	int x=shared_mem_open("hui", getO_CREAT_ORDWR());
	return 0;
}
