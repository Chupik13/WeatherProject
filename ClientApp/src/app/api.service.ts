import {Inject, Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse, HttpParams} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {FilterOptions} from "./models/FilterOptions";
import {WeatherUnit} from "./models/WeatherUnit";

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseUrl = baseUrl;
  }

  getWeatherArchive(pageNumber: number, pageSize: number, filters?: FilterOptions): Observable<WeatherUnit[]> {
    const url = `${this.baseUrl}weather/GetWeatherArchive`;
    let params = new HttpParams()
      .append('pageNumber', pageNumber.toString())
      .append('pageSize', pageSize.toString());

    if (filters) {
      Object.keys(filters).forEach((key) => {
        let value = filters[key];
        if (value) {
          params = params.append(key, value.toString());
        }
      });
    }

    return this.http.get<WeatherUnit[]>(url, {params}).pipe(
      catchError(this.handleError)
    );
  }

  clearWeatherData(): Observable<any> {
    const url = `${this.baseUrl}weather/ClearWeatherArchive`;

    return this.http.post(url, null).pipe(
      catchError(this.handleError)
    );
  }

  getTotalWeatherArchive(filters?: FilterOptions): Observable<number> {
    const url = `${this.baseUrl}weather/GetTotalWeatherArchive`;
    let params = new HttpParams();

    if (filters) {
      Object.keys(filters).forEach((key) => {
        let value = filters[key];
        if (value) {
          params = params.append(key, value.toString());
        }
      });
    }

    return this.http.get<number>(url, {params}).pipe(
      catchError(this.handleError)
    );
  }

  sendWeatherArchiveUrl(): string {
    return `${this.baseUrl}weather/UploadWeatherArchive`;
  }

  private handleError(error: HttpErrorResponse) {
    if (error.status === 0) {
      console.error('An error occurred:', error.error);
    } else {
      console.error(`Backend returned code ${error.status}, body was: `, error.error);
    }
    return throwError(() => new Error('Something bad happened; please try again later.'));
  }
}
