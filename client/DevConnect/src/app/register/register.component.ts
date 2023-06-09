import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private accountService:AccountService){}

  model: any={};
  
  @Output() cancelRegister = new EventEmitter();
  ngOnInit(): void {
    
  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: ()=>{        
        this.cancel();
      },
      error: (error: any) =>console.log(error)
    });
  }

  cancel()
  {
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }

}
