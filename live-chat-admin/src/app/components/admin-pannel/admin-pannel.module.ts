import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashBoardComponent } from './dash-board/dash-board.component';


// import { SideNavComponent } from './side-nav/side-nav.component';
// import { DashboardComponent } from './dashboard/dashboard.component';
// import { AddPostComponent } from './add-post/add-post.component';
// import { ViewPostComponent } from './view-post/view-post.component';
import { RouterModule, Routes } from '@angular/router';
import {MatButtonModule} from '@angular/material/button';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatListModule} from '@angular/material/list';
import {MatIconModule} from '@angular/material/icon';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatDatepickerModule} from '@angular/material/datepicker';

import {MatInputModule} from '@angular/material/input';
import {MatToolbarModule} from '@angular/material/toolbar';

import {  FormsModule, ReactiveFormsModule} from '@angular/forms';
import {MatCardModule} from '@angular/material/card';


import { ViewChild} from '@angular/core';
import {MatAccordion, MatExpansionModule} from '@angular/material/expansion';

import {provideNativeDateAdapter} from '@angular/material/core';
import { SideNavComponent } from './side-nav/side-nav.component';
import { ChatComponent } from './chat/chat.component';


const routes: Routes = [

  {
   path:'admin-pannel',
   component:SideNavComponent,
   children:[
    {
      path:'',
      redirectTo:'chat',
      pathMatch:'full'
    },
    {
      path:'dashboard',
      component:DashBoardComponent
    },
    {
      path:'chat',
      component:ChatComponent
    }
   ]
  },
 


];


@NgModule({
  declarations: [
    DashBoardComponent,
    SideNavComponent,
    ChatComponent
  ],
  providers: [provideNativeDateAdapter()],
  imports: [
    CommonModule,
   
    RouterModule.forChild(routes),
    MatSidenavModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatInputModule,
    MatToolbarModule,
    ReactiveFormsModule,
    FormsModule,
    MatCardModule,
  
    MatExpansionModule,
  ]
})
export class AdminPannelModule { }
