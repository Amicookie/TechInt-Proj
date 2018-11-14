import { Component, OnInit } from '@angular/core';
import { FileService } from '../file.service';
import { HttpClient } from '@angular/common/http';
import { ObjectUnsubscribedError } from 'rxjs';

@Component({
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.css']
})
export class FileComponent implements OnInit {

  public files = [];
  public errorMsg;
  file_name = "";
  file_content = "";
  current_file_id = 0;

  BreakException = {};
  
  constructor(private _filesService: FileService) { }

  ngOnInit() {
    this._filesService.getFiles()
        .subscribe(data => this.files = data,
                    error => this.errorMsg = error);
  }

  // ngOnDestroy() {
  //   this._filesService.unsubscribe()
  // }

  navigateToFile(clicked_file_id){
    try{
      this.files.forEach(element => {
        if (element.file_id == clicked_file_id) {
          this.current_file_id = clicked_file_id;
          this.file_name = element.file_name;
          this.file_content = element.file_content;
          throw this.BreakException;
        }
      });
    } catch (e) {
      if (e !== this.BreakException) throw e;
    }
  }

  createFile(file_name, file_content){
    this._filesService.createFile(file_name, file_content, 1);
  }

  editFile(file_id, file_name, file_content){
    this._filesService.editFile(file_id, file_name, file_content, 1);
  }

}
