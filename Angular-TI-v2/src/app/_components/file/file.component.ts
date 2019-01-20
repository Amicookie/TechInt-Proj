import { Component, OnInit, OnDestroy } from '@angular/core';
import { FileService } from '../../_services/file.service';
import { showToast } from '../../toaster-helper';
import { WebsocketService } from 'src/app/_services/websocket.service';



@Component({
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.css',
              '../../app.component.css']
})
export class FileComponent implements OnInit, OnDestroy {

  public files: any;
  public errorMsg;
  file_name = "";
  file_content = "";
  current_file_id = 0;

  _navigateToFile = false;
  editable = false;
  _file_locked: boolean;
  _locked_file_id: number;
  _locked_user_id: number;

  BreakException = {};
  interval: any;
  
  constructor(private _filesService: FileService, private webSocketService: WebsocketService) { 
    this.webSocketService._file_locked.subscribe(
      value => {
        this._file_locked = value;
      }
    );
    this.webSocketService._locked_file_id.subscribe(
      value => {
        this._locked_file_id = value;
      }
    )
    this.webSocketService._locked_user_id.subscribe(
      value => {
        this._locked_user_id = value;
      }
    )

    this.webSocketService._file_changed.subscribe(
      value => {
        if (value.file_id !== -1) {
          // this.files.forEach(file => {
          //   console.log(file);
          //   if(file.file_id == value.file_id) {
          //     console.log('weszlo do ifa filechanged w petli po plikach');
          //     this._filesService.getFile(value.file_id).subscribe(data => {
          //       file = data;
          //       console.log(file);
          //       this.webSocketService._file_changed.next({file_id:-1});
          //     });
          //   }
          // })
          this._filesService.getFiles().subscribe(data=>{
            this.files = data;
          });
          this.webSocketService._file_changed.next({file_id:-1});
        }
      }
    )
    this.webSocketService._file_added.subscribe(
      value => {
        if (value.file_id !== -1) {

          console.log('weszlo do ifa');
          console.log('value.file_id' + value.file_id);
          //console.log('value' + value);
          this._filesService.getFiles().subscribe(data=>{
            this.files = data;
          });
          this.webSocketService._file_added.next({file_id: -1});
          console.log('wartosc file_added po .next(-1)'+value);
        }
      }
    )
  }
  ngOnInit() {
    this._filesService.getFiles()
    .subscribe(data => this.files = data,
                error => this.errorMsg = error);
  }

  ngOnDestroy() {

  }

  editable_switch(){
    if (this.editable == true){
     this.webSocketService.emitEventOnFileLocked(localStorage.getItem('user_id'), localStorage.getItem('currentUser'), this.file_name, this.current_file_id);
    } else {
      this.webSocketService.emitEventOnFileUnlocked(localStorage.getItem('currentUser'), this.file_name, this.current_file_id);
     }
  }

  navigateToFile(clicked_file_id){
    console.log("CLICKED!!")
    try{
      this.files.forEach(element => {
        if (element.file_id == clicked_file_id) {
          this.current_file_id = clicked_file_id;
          this.file_name = element.file_name;
          this.file_content = element.file_content;
          this._navigateToFile = true;
          throw this.BreakException;
        }
      });
    } catch (e) {
      if (e !== this.BreakException) throw e;
    }
  }

  backToList(){

    //+ operator changes string into number <3
    if(this.editable && +localStorage.getItem('user_id')==this._locked_user_id){
       this.webSocketService.emitEventOnFileUnlocked(localStorage.getItem('currentUser'), this.file_name, this.current_file_id);
       this.editable = !this.editable
    }
    this.current_file_id = 0;
    this.file_name = "";
    this.file_content = "";
    this._navigateToFile = false;
  }

  createFile(file_name, file_content){
    this._filesService.createFile(file_name, file_content, localStorage.getItem('user_id'));
    //wyczyść zawartość textarea jeśli utworzono xd
  }

  editFile(file_id, file_name, file_content){
    this._filesService.editFile(file_id, file_name, file_content, localStorage.getItem('user_id'))
  }

}
