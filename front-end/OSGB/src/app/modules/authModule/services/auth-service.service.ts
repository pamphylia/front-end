import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { ITokenResponse } from '../../../common/security/ITokenResponse';
import { map, retry } from 'rxjs/operators';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AuthService {
  constructor(private httpClient: HttpClient) { }
  login(email: string, password: string): Observable<boolean> {
    const body = new HttpParams()
      .set('username', email)
      .set('password', password);
      return this.httpClient.post(
        'https://osgb.azurewebsites.net/api/token',
        body.toString(),
        {
          headers: new HttpHeaders()
            .set('Content-Type', 'application/x-www-form-urlencoded')
        }).pipe(retry(3), map((response: ITokenResponse) => {
          if (response.statusCode &&  response.statusCode === 401) {
           return false;
          } else {
            return true;
          }
        }));
  }

  logout() {
    localStorage.removeItem('currentUser');
  }
}
