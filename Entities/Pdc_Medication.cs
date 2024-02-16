using System;

namespace OntrackDb.Entities
{
    public class Pdc_Medication
    {
        public int Id { get; set; }
        public string value_set_id { get; set; }
        public string value_set_subgroup { get; set; }
        public string value_set_item { get; set; }
        public string code_type { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string route { get; set; }
        public string dosage_form { get; set; }
        public string ingredient { get; set; }
        public string strength { get; set; }
        public string units { get; set; }
        public string from_date { get; set; }
        public string thru_date { get; set; }
        public string attribute_type { get; set; }
        public string attribute_value { get; set; }
        public string category{ get; set; }

        public Boolean IsExclude { get; set; }
    }
}
