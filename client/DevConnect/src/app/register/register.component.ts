import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private accountService:AccountService,
    private toastr:ToastrService, 
    private fb : FormBuilder,
    private router: Router){}

  
  registerForm: FormGroup = new FormGroup({});  
  @Output() cancelRegister = new EventEmitter();
  validationErrors: string[] | undefined;

  ngOnInit(): void {
    this.initializeRegistrationForm();
  }

  matchValues(matchTo:string):ValidatorFn{
    return(control:AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ?null:{notMatching:true}
    }
  }

  initializeRegistrationForm(){
    this.registerForm = this.fb.group({
      Gender : ['male'],
      UserName : ['',Validators.required],
      FullName : ['',Validators.required],
      DateOfBirth : ['',Validators.required],
      City : ['',Validators.required],
      Country : ['',Validators.required],
      Password:  ['',[Validators.required, Validators.minLength(8), Validators.maxLength(15)]],
      ConfirmPassword: ['',[Validators.required, this.matchValues('Password')]],
    });
    this.registerForm.controls['Password'].valueChanges.subscribe({
      next:() => this.registerForm.controls['ConfirmPassword'].updateValueAndValidity()
    })
  }

  register(){
    const dob = this.getDateOnly(this.registerForm.controls['DateOfBirth'].value);
    const values={...this.registerForm.value, DateOfBirth:dob};
    console.log(values);
    this.accountService.register(values).subscribe({
      next: ()=>{        
        this.router.navigateByUrl('/members');
      },
      error: error =>{
        //this.toastr.error(error.error),
        this.validationErrors = error
      }
    });
    
  }

  cancel()
  {
    console.log("cancelled");
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob:string | undefined){
    if(!dob) return;
    let retValDob = new Date(dob);
    return new Date(retValDob.setMinutes(retValDob.getMinutes() - retValDob.getTimezoneOffset()))
    .toISOString().slice(0,10);
  }

}
