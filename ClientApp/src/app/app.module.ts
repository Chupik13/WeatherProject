import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';


import {AppComponent} from './app.component';
import {MainAppComponent} from './components/main-app/main-app.component';
import {NZ_I18N, ru_RU} from 'ng-zorro-antd/i18n';
import {registerLocaleData} from '@angular/common';
import ru from '@angular/common/locales/ru';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {NzUploadModule} from "ng-zorro-antd/upload";
import {NzButtonModule} from 'ng-zorro-antd/button';
import {NzMessageModule} from "ng-zorro-antd/message";
import {WeatherListComponent} from './components/weather-list/weather-list.component';
import {NzSelectModule} from 'ng-zorro-antd/select';
import {NzTableModule} from "ng-zorro-antd/table";
import {NzIconModule} from "ng-zorro-antd/icon";
import {NzPopconfirmModule} from "ng-zorro-antd/popconfirm";
import {NzSpaceModule} from "ng-zorro-antd/space";
import {NzDatePickerModule} from "ng-zorro-antd/date-picker";
import {PanelComponent} from './components/panel/panel.component';
import {NzTypographyModule} from "ng-zorro-antd/typography";
import {NzEmptyModule} from "ng-zorro-antd/empty";

registerLocaleData(ru);

@NgModule({
  bootstrap: [AppComponent],
  declarations: [
    AppComponent,
    MainAppComponent,
    WeatherListComponent,
    PanelComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: MainAppComponent, pathMatch: 'full'},
    ]),
    BrowserAnimationsModule,
    NzUploadModule,
    NzButtonModule,
    NzMessageModule,
    NzSelectModule,
    NzTableModule,
    NzIconModule,
    NzPopconfirmModule,
    NzSpaceModule,
    NzDatePickerModule,
    NzTypographyModule,
    NzEmptyModule
  ],
  providers: [
    {provide: NZ_I18N, useValue: ru_RU}
  ]
})
export class AppModule {
}
