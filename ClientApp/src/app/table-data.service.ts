import {Injectable} from '@angular/core';
import {Subject} from "rxjs";
import {FilterOptions} from "./models/FilterOptions";

@Injectable({
  providedIn: 'root'
})
export class TableDataService {
  private refreshTableSource = new Subject<void>();
  refreshTable$ = this.refreshTableSource.asObservable();
  private updateFilterOptionsSource = new Subject<FilterOptions>();
  updateFilterOptions$ = this.updateFilterOptionsSource.asObservable();

  constructor() {
  }

  refreshTable(): void {
    this.refreshTableSource.next();
  }

  updateFilterOptions(filterOptions: FilterOptions): void {
    this.updateFilterOptionsSource.next(filterOptions);
  }
}
