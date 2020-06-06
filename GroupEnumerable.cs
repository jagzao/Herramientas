using System;
using System.Collections.Generic;
using System.Linq;

namespace Tool
{
    public static class GroupEnumerable
    {
        /// <summary>
        /// Ingresas tu datos a ordenar en arbol
        /// Te regresa un ResponseDto, con el arbol
        /// </summary>
        /// <param name="dataq"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ResponseDto ToTree<T>
            (IEnumerable<object> dataq,
            string campopadre,
            string msg = "") where T : class, new()
        {
            var r = new ResponseDto();
            var datos = new List<OptionTree>();
            try
            {
                if (dataq != null)
                {
                    var type = typeof(T);
                    var properties = type.GetProperties();

                    var qu = dataq.ToList();
                    var l = (from d in qu
                             select new OptionTree
                             {
                                 Objeto = d,
                                 id = d.GetPropValue<int>(properties[0].Name),
                                 idParent = d.GetPropValue<int?>(campopadre)
                             }).ToList();
                    datos = BuildTreeAndReturnRootNodes(l);

                    return r.ToExito(datos);
                }
                else
                    return r.ToError("No hay datos para mostrar ;(");
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    Exito = false,
                    Datos = datos,
                    Mensaje = ex.ToString()
                };
            }
        }

        public static List<OptionTree> BuildTreeAndReturnRootNodes(List<OptionTree> flatItems)
        {
            flatItems = flatItems.GroupBy(c => c.id, (key, c) => c.FirstOrDefault()).ToList();
            var groupParents = flatItems.Where(s => s.idParent == s.id).ToList();

            if (groupParents.Any())
            {
                groupParents.ForEach(s => s.idParent = null);
            }

            var byIdLookup = flatItems.ToLookup(i => i.id);
            foreach (var item in flatItems)
            {
                if (item.idParent != null)
                {
                    var parent = byIdLookup[item.idParent.Value].First();
                    parent.children.Add(item);
                }
                else
                {

                }
            }
            var flatI = flatItems.Where(i => i.idParent == null).ToList();
            return flatI;
        }

        public class OptionTree
        {
            public OptionTree()
            {
                children = new List<OptionTree>();
            }
            public object Objeto { get; set; }
            public int id { get; set; }
            public string label { get; set; }
            public int level { get; set; }
            public int? idParent { get; set; }
            public List<OptionTree> children { get; set; }
        }
    }
}
