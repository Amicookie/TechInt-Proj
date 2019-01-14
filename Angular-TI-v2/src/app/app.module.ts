import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { FileComponent } from './_components/file/file.component';
import { UserComponent } from './_components/user/user.component';
import { UiModule } from './_components/ui/ui.module';

import { HttpClientModule, HttpClientJsonpModule } from '@angular/common/http';
import { LoginComponent } from './_components/login/login.component';

import { WebsocketService } from './_services/websocket.service';
import { ChatService } from './_services/chat.service';

import { FormsModule } from '@angular/forms';
import { DataSharingService } from './_services/data-sharing.service';

@NgModule({
  declarations: [
    AppComponent,
    FileComponent,
    UserComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    HttpClientJsonpModule,
    AppRoutingModule,
    UiModule,
    FormsModule
  ],
  providers: [
    ChatService,
    WebsocketService,
    DataSharingService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
