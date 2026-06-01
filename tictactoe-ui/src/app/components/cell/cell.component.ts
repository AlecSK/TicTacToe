import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CellMark } from '../../models/models';

@Component({
  selector: 'app-cell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cell.component.html',
  styleUrl: './cell.component.scss'
})
export class CellComponent {
  @Input() mark: CellMark = '-';
  @Input() disabled = false;
  @Input() isWinning = false;
  @Input() index = 0;

  @Output() cellClicked = new EventEmitter<number>();

  onClick(): void {
    if (!this.disabled && this.mark === '-') {
      this.cellClicked.emit(this.index);
    }
  }
}
