import { Injectable } from '@angular/core';
import * as io from 'socket.io-client';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../environments/environment';
import { IFile } from '../_models/file';
import { IUser } from '../_models/user';
import { showToast } from '../toaster-helper';

@Injectable({
  providedIn: 'root'
})

export class WebsocketService {

  private socket; // socket connects to socket.io server

  constructor() {
    
    //this.socket = io.connect(environment.ws_url);

  }

  // connect() {
  //   this.socket.on('connect', function() {
  //       this.socket.send('User has connected!');
  //   })
  // }
  


  connect(): Subject<MessageEvent>{
    this.socket = io.connect(environment.ws_url);

    let observable = new Observable(observer => {
      this.socket.on('connect', () => {
        this.socket.emit('User connected');
      })

      this.socket.on('message', (data)=> {
        console.log("Received a msg from server");
        observer.next(data);
      })
      return() => {
        this.socket.disconnect();
      }
    })

    let observer = {
      next: (data: Object) => {
        this.socket.emit('message', JSON.stringify(data));
      },
    }
    return Subject.create(observer,observable);
  }


   //obsługa eventów

   emitEventOnFileSaved(fileSaved) {
     this.socket.emit('fileSaved', fileSaved);
   }

   emitEventOnFileLocked(fileLocked){
     this.socket.emit('fileLocked', fileLocked);
   }

  //  emitEventOnFileUpdated(fileUpdated){
  //   this.socket.emit('fileUpdated', fileUpdated);
  // }
  emitEventOnFileUpdated(file_name, user_login){
    this.socket.emit('fileUpdated', file_name, user_login);
  }

  // consume File Saved ---- create new file
  consumeEventOnFileSaved(){
    const self = this;
    // this.socket.on('fileSaved', function(file: IFile){
    //   self.toastr.success('New File Saved!', 'A File \"'+ file.file_name + '\" has just been shared!')
    // });
    this.socket.on('fileSaved', function(file_name){
      showToast('A File \"'+ file_name + '\" has just been shared!');
      //self.toastr.success({title:'New File Saved!', msg:'A File \"'+ file_name + '\" has just been shared!'});
    });

  }

  consumeEventOnFileLocked(){
    const self = this;
    // this.socket.on('fileLocked', function(file: IFile, user: IUser){
    //   self.toastr.info('File Locked!', 'File \"'+ file.file_name +'\" locked by '+ user.user_login+'!')
    // });
    this.socket.on('fileLocked', function(file: IFile, user: IUser){
      showToast('File \"'+ file.file_name +'\" locked by '+ user.user_login+'!');
        //self.toastr.info({title:'File Locked!', msg:'File \"'+ file.file_name +'\" locked by '+ user.user_login+'!'});
      });
  }

  consumeEventOnFileUpdated(){
    const self = this;
    // this.socket.on('fileUpdated', function(file: IFile, user: IUser){
    //   self.toastr.info('File Updated!', 'File \"'+ file.file_name +'\" updated by '+ user.user_login+'!')
    // });
    this.socket.on('fileUpdated', function(file_name, user_login){
      showToast('File \"'+ file_name +'\" updated by '+ user_login+'!');
       //self.toastr.info({title:'File Updated!', msg:'File \"'+ file_name +'\" updated by '+ user_login+'!'});
    });
  }

}