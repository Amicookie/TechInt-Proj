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

  public files = [];
  public errorMsg;
  file_name = "";
  file_content = "";
  current_file_id = 0;

  _navigateToFile = false;
  editable = false;
  _file_locked: boolean;
  //_disabled = this.webSocketService._disabled;

  BreakException = {};
  interval: any;
  
  constructor(private _filesService: FileService, private webSocketService: WebsocketService) { 
    this.webSocketService._file_locked.subscribe(
      value => {
        this._file_locked = value;
      }
    );
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
     this.webSocketService.emitEventOnFileLocked(localStorage.getItem('currentUser'), this.file_name);
    } else {
      this.webSocketService.emitEventOnFileUnlocked(localStorage.getItem('currentUser'), this.file_name);
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
    this.current_file_id = 0;
    this.file_name = "";
    this.file_content = "";
    this._navigateToFile = false;
  }

  createFile(file_name, file_content){
    this._filesService.createFile(file_name, file_content, localStorage.getItem('user_id'))
  }

  editFile(file_id, file_name, file_content){
    this._filesService.editFile(file_id, file_name, file_content, localStorage.getItem('user_id'))
  }

}
