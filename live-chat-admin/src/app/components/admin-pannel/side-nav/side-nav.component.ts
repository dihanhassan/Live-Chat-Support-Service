import { Component ,inject} from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../../../services/chat.service';
@Component({
  selector: 'app-side-nav',
  templateUrl: './side-nav.component.html',
  styleUrl: './side-nav.component.scss'
})
export class SideNavComponent {
  sideBarOpen = true;

  
  isConnected : boolean = false;
  constructor(private router: Router,
    public chatService : ChatService
    ) { 
     
    }

  connectedUsers: any[] = [];
  
  ngOnInit(): void {
    this.hubConnection();
  }

  startChatWithUser(user: { item1: string, item2: string, item3: string }) {
    const queryParams = {
      name: user.item1,
      id: user.item2,
      email: user.item3
    };
    console.log("here")
    const adminMail = sessionStorage.getItem("user");
    if(adminMail){
      console.log("admin mail")
      
      this.chatService.removeOtherAdmin(adminMail,user.item2,1);
      this.router.navigate(['admin-pannel/chat'], { queryParams });
    }else{
      console.log('admin mail not found');
    }
    
  }
  toggleSidebar() {
    this.sideBarOpen = !this.sideBarOpen;
  }
  logout(){
    
  }

  hubConnection(){
    console.log('dash board');
    this.chatService.joinRoom("admin","admin@gmail.com",true).then(()=>{
      console.log('joined');
    }).catch(()=>{
      console.log('error');
    });
    this.chatService.connectedUsers$.subscribe(res=>{
      
      this.connectedUsers = res;
      console.log(this.connectedUsers);

    });
  }
}
