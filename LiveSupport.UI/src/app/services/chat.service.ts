import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  public connection : any = new signalR.HubConnectionBuilder().withUrl("https://localhost:7112/chat").withAutomaticReconnect()
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
    this.connection.on("ConnectedUsers",(users:any)=>{
      console.log(users);
      console.log("connected users");
     // this.connectedUsers$.next(users);
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

  // page reload 

 


  // join Room
  public async joinRoom(name: string, email:string,siteID:Number){
    console.log(name); 
    
    return this.connection.invoke("JoinRoom", { name: name, email: email, siteID: siteID })
    .then((roomID: string) => {
        console.log(roomID);
        sessionStorage.setItem("roomID", roomID);
    })
    .catch((error: any) => {
        console.error("Error joining room:", error);
    });

  }

  // send message
  public async sendMessage(message:string){
    const roomID = sessionStorage.getItem("roomID");
    return this.connection.invoke("SendMessage",{message,roomID});
  }

  // leave room

  public async leaveChat(){
    return this.connection.stop();
  }

}
