import { Component, OnInit, OnDestroy } from '@angular/core';
import { FileService } from '../../_services/file.service';
import { showToast } from '../../toaster-helper';


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

  BreakException = {};
  interval: any;
  
  constructor(private _filesService: FileService) { 

  }
  ngOnInit() {
    this._filesService.getFiles()
    .subscribe(data => this.files = data,
                error => this.errorMsg = error);
  }

  ngOnDestroy() {

  }

  navigateToFile(clicked_file_id){
    console.log("CLICKED!!")
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
    //console.log(this._filesService.createFile(file_name, file_content, 1));
    // if(){
    //   this.files.push(this._filesService.getFile(this.files.length-1).subscribe());
    // }
    this._filesService.createFile(file_name, file_content, 1)
    //this.refreshData();
  }

  editFile(file_id, file_name, file_content){
    this._filesService.editFile(file_id, file_name, file_content, 1)
  }



  // myFunction() {
  //   // Get the snackbar DIV
  //   var x = document.getElementById("snackbar");
  
  //   // Add the "show" class to DIV
  //   x.className = "show";
  
  //   // After 3 seconds, remove the show class from DIV
  //   setTimeout(function(){ x.className = x.className.replace("show", ""); }, 3000);
  // }

}
