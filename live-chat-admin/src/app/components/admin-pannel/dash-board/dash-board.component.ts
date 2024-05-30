import { Component ,OnInit,inject} from '@angular/core';
import { ChatService } from '../../../services/chat.service';
@Component({
  selector: 'app-dash-board',
  templateUrl: './dash-board.component.html',
  styleUrl: './dash-board.component.scss'
})
export class DashBoardComponent implements OnInit {
  connected:boolean = false;
  
  chatService = inject(ChatService);
  
  constructor() {  
   
   }
  index:number = 1;
  connectedUsers: string[] = [];
  users:any[] = [];
  ngOnInit(): void {
  
    
  }

}
