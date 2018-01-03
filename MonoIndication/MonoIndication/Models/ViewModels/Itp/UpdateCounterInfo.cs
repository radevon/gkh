using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonoIndication
{
    public class UpdateCounterInfo
    {
        // дата получения значений которые скорректированы
        public DateTime RecvDate { get; set; }
        // значение теплоэнергии по счетчику новому (фактически полученое)
        public double NewHeat { get; set; }
        // значение теплоэнергии по счетчику на момент замены (ввод вручную) <=NewHeat
        public double NewHeatOnRemove { get; set; }
        // значение теплоэнергии по счетчику предыдущее (старый счетчик) (фактически полученое)
        public double OldHeat { get; set; }
        // значение теплоэнергии по счетчику старое снятое вручную при замене(старый счетчик) >=OldHeat
        public double OldHeatOnRemove { get; set; }

        // разность между показаниями
        public double NewHeatSubstract
        {
            get
            {
                return NewHeat - NewHeatOnRemove;
            }
        }

        // разность между показаниями
        public double OldHeatSubstract
        {
            get
            {
                return OldHeatOnRemove - OldHeat;
            }
        }

        // фактическое значение коррекции которое ложится в поле БД
        public double Correction
        {
            get { return OldHeatOnRemove - NewHeatOnRemove; }
        }

        // Фактические расход между двумя снятыми показаниями после скорректированных значений
        public double FactLoseHeat
        {
            get { return NewHeat + Correction - OldHeat; }
        }

    }
}