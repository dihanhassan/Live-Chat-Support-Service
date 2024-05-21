import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  public connection : any = new signalR.HubConnectionBuilder().withUrl("https://localhost:7112/chat")
  .configureLogging(signalR.LogLevel.Information)
  .build();

  public messages$ = new BehaviorSubject<any>([]);
  public connectedUsers$ = new BehaviorSubject<string[]>([]);
  public messages: any[] = [];

  public users : string[] = [];

  constructor() {
    this.start()
    console.log(this.connection);
    this.connection.on("ReceiveMessage", (user:string ,message:string,messageTime:string)=>{
      console.log(user);
      console.log(message);
      console.log(messageTime);
      this.messages = [...this.messages,{user,message,messageTime}];
      this.messages$.next(this.messages);
    });
    this.connection.on("ConnectedUser",(users:any)=>{
      console.log(users);
      console.log("connected users");
      this.connectedUsers$.next(users);
    });

   }

  // start connection
  public async start (){
    try {
      await this.connection.start();
      console.log("connected");
    } catch (error) {
      console.log(error);
     
    }
  }

  // join Room
  public async joinRoom(user: string, room: string){
    console.log(user,room); 
    return this.connection.invoke("JoinRoom", {user,room});
  }

  // send message
  public async sendMessage(message:string){
    return this.connection.invoke("SendMessage",message);
  }

  // leave room

  public async leaveChat(){
    return this.connection.stop();
  }

}
