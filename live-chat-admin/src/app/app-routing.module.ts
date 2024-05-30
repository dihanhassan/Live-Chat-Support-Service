import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  
  {
    path:'',
    loadChildren:()=>import('./components/admin-pannel/admin-pannel.module').then(m=>m.AdminPannelModule)
  },
  {
    path:'admin-pannel',
    loadChildren:()=>import('./components/admin-pannel/admin-pannel.module').then(m=>m.AdminPannelModule)
  }
  
  
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
