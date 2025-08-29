import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';

import { map, tap } from 'rxjs';
import { Address, User } from '../../shared/models/user';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);
    private signalrService = inject(SignalrService);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);
    return this.http.post<User>(this.baseUrl + 'login', values, { params});
  }

  register(values: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', values);
  }

  getUserInfo() {
  console.log("tony song ttt");
   console.log(this.baseUrl);

     console.log("tony song tttooo");
    return this.http.get<User>(this.baseUrl + 'account/user-info').pipe(
        map(user => {
          this.currentUser.set(user);
          return user;
        })
      );
  }

   logout() {
    return this.http.post(this.baseUrl + 'account/logout', {}).pipe(
      tap(() => this.signalrService.stopHubConnection())
    )
  }
  updateAddress(address: Address) {
    return this.http.post(this.baseUrl + 'account/address', {});
  }

  getAuthState() {
    return this.http.get<{isAuthenticated: boolean}>(this.baseUrl + 'account/auth-status');
  }

}
