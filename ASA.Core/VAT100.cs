using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASA.Core
{
    [Table("VATInfo")]
    public class VAT100
    {
        private int _id;
    private string _box1;
    private string _box2;
    private string _box3;
    private string _box4;
    private string _box5;
    private string _box6;
    private string _box7;
    private string _box8;
    private string _box9;
    private string _box10;
    private DateTime _lastUpdated;
    private bool _finalReturn;

   // public int PeriodId { get; set; }
    //[ForeignKey("PeriodId")]
    public virtual PeriodData PeriodData { get; set; }

        [Key, ForeignKey("PeriodData")] //one to one relation ship 
    public int Id
    {
      get
      {
        return this._id;
      }
      set
      {
        this._id = value;
      }
    }
    public bool FinalReturn
    {
      get
      {
        return this._finalReturn;
      }
      set
      {
        this._finalReturn = value;
      }
    }
    public string Box1
    {
      get
      {
        return this._box1;
      }
      set
      {
        this._box1 = value;
      }
    }
    public string Box2
    {
      get
      {
        return this._box2;
      }
      set
      {
        this._box2 = value;
      }
    }
    public string Box3
    {
      get
      {
        return this._box3;
      }
      set
      {
        this._box3 = value;
      }
    }

    public string Box4
    {
      get
      {
        return this._box4;
      }
      set
      {
        this._box4 = value;
      }
    }

    public string Box5
    {
      get
      {
        return this._box5;
      }
      set
      {
        this._box5 = value;
      }
    }

    public string Box6
    {
      get
      {
        return this._box6;
      }
      set
      {
        this._box6 = value;
      }
    }

    public string Box7
    {
      get
      {
        return this._box7;
      }
      set
      {
        this._box7 = value;
      }
    }

    public string Box8
    {
      get
      {
        return this._box8;
      }
      set
      {
        this._box8 = value;
      }
    }

    public string Box9
    {
      get
      {
        return this._box9;
      }
      set
      {
        this._box9 = value;
      }
    }

    public string Box10
    {
      get
      {
        return this._box10;
      }
      set
      {
        this._box10 = value;
      }
    }

    public Decimal Box10AsNumber
    {
      get
      {
        if (this._box10.Equals(""))
          return new Decimal(0);
        return Decimal.Parse(this._box10);
      }
    }

    public DateTime LastUpdated
    {
      get
      {
        return this._lastUpdated;
      }
      set
      {
        this._lastUpdated = value;
      }
    }

    public VAT100()
    {
      //this._id = -1;
      //this._box1 = new Decimal(0);
      //this._box2 = new Decimal(0);
      //this._box3 = new Decimal(0);
      //this._box4 = new Decimal(0);
      //this._box5 = new Decimal(0);
      //this._box6 = new Decimal(0);
      //this._box7 = new Decimal(0);
      //this._box8 = new Decimal(0);
      //this._box9 = new Decimal(0);
      //this._box10 = "";
      //this._lastUpdated = Constants.DefaultDate;
     // this._finalReturn = false;
    }

    //public string GetBox5ForFBI(string format)
    //{
    //  Decimal num = new Decimal(0);
    //  return (!(this._box5 < new Decimal(0)) ? this._box5 : new Decimal(0) - this._box5).ToString(format);
    //}

    //public void UpdaetBoxValue(int boxId, Decimal value)
    //{
    //  switch (boxId)
    //  {
    //    case 1:
    //      this._box1 = value;
    //      break;
    //    case 2:
    //      this._box2 = value;
    //      break;
    //    case 3:
    //      this._box3 = value;
    //      break;
    //    case 4:
    //      this._box4 = value;
    //      break;
    //    case 5:
    //      this._box5 = value;
    //      break;
    //    case 6:
    //      this._box6 = value;
    //      break;
    //    case 7:
    //      this._box7 = value;
    //      break;
    //    case 8:
    //      this._box8 = value;
    //      break;
    //    case 9:
    //      this._box9 = value;
    //      break;
    //    case 10:
    //      this._box10 = value.ToString();
    //      break;
    //  }
    //}
  }
    
}
