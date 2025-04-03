import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'vietnameseDatetime'
})
export class VietnameseDatetimePipe implements PipeTransform {

  private readonly weekdayMap: Record<number, string> = {
    0: 'Chủ Nhật',
    1: 'Thứ Hai',
    2: 'Thứ Ba',
    3: 'Thứ Tư',
    4: 'Thứ Năm',
    5: 'Thứ Sáu',
    6: 'Thứ Bảy',
  };

  transform(value: Date | string | null): string {
    if (!value) return '';

    const date = new Date(value);
    const day = date.getDay();
    const d = date.getDate();
    const m = date.getMonth() + 1;
    const y = date.getFullYear();
    const h = date.getHours();
    const min = date.getMinutes();
    const s = date.getSeconds();

    const weekday = this.weekdayMap[day];

    return `${weekday}, ngày ${d} tháng ${m} năm ${y} lúc ${h} giờ ${min} phút ${s} giây`;
  }
}
