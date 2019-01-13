import { Component, OnInit } from '@angular/core';
import { showToast } from '../../../toaster-helper';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css',
              '../../../app.component.css',
              './toaster.css']
})
export class LayoutComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
