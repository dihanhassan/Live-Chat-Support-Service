import { Injectable, OnInit } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { Message } from '../models/message';

@Injectable({
  providedIn: 'root'
})
export class ChatService implements OnInit {

  public connection: any;
  public messages$ = new BehaviorSubject<Message[]>([]);
  public connectedUsers$ = new BehaviorSubject<any>([]);
  public messages: any[] = [];
  public users: string[] = [];

  constructor() {
    this.initializeConnection();
  }
  ngOnInit(): void {
    this.initializeConnection();
  }
  // Initialize SignalR connection
  private initializeConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7112/chat")
      .configureLogging(signalR.LogLevel.Information)
      .build();

    
      this.start();
    this.connection.on("ConnectedUsers", (allUsers: { item1: string, item2: string }[]) => {
      console.log(allUsers);
      this.connectedUsers$.next(allUsers);
    
      // Extracting names and IDs from each object
      const names = allUsers.map(user => user.item1);
      const ids = allUsers.map(user => user.item2);
    
      console.log(names);
      console.log(ids);
    });
    this.messageUpdate();

    // Start the connection
    
  }

  public  messageUpdate(){
    this.connection.on("ReceiveMessage", (message: Message) => {
    
      console.log(message);
      this.messages = [...this.messages,message];
      this.messages$.next(this.messages);
      console.log(this.messages)
    });
  }

  // Start connection
  public async start() {
    try {
      await this.connection.start();
      console.log("connected");

     
    } catch (error) {
      console.log(error);
    }
  }

  // Join Room
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

  // Send message
  public async sendMessage(message:string){
    const roomID = sessionStorage.getItem("roomID");
    return this.connection.invoke("SendMessage",{message,roomID});
  }

  // Leave room
  public async leaveChat() {
    try {
      await this.connection.stop();
      console.log('left chat');
    } catch (error) {
      console.log(error);
    }
  }

}
