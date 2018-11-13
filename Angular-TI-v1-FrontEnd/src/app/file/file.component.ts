import { Component, OnInit } from '@angular/core';
import { FileService } from '../file.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.css']
})
export class FileComponent implements OnInit {

  public files = [];
  public errorMsg;
  
  constructor(private _filesService: FileService) { }

  ngOnInit() {

    this._filesService.getFiles()
        .subscribe(data => this.files = data,
                    error => this.errorMsg = error);
  }

}
