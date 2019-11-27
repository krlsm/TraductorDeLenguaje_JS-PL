using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
//using AnalizadorLexico;



namespace ProcesadorDeLenguaje_JS_PL
{
    public class AnalizadorSintactico
    {
        
        /*Este es un analizador sintáctico LR(1)
         Contiene 
            Una Pila
            Dos Tablas: 
                    Tabla Accion
                    Tabla GOTO
        */


        private List<string[]> tablaAccion;
        private Dictionary<string, int> columnaAccion; // Devuelve la posicion j donde esta el token en la tablaAccion ej tablaAccion[numero estado,id] transforma el id al indice de turno.
        private List<string[]> tablaGoto;
        private Dictionary<string, int> columnaGoto;
        private List<(string,int)> reglas = new List<(string Antecedente,int numDeConsecuentes)>();
        private AnalisisLexico alex;

        public AnalizadorSintactico(AnalisisLexico alex,string pathTablaAccion,string pathTablaGoto,string pathNumeroConsecuentes)
        {
            this.alex = alex; // recibimos el analizador lexico. 
             tablaAccion = CrearTabla(pathTablaAccion,columnaAccion);
             tablaGoto = CrearTabla(pathTablaGoto,columnaGoto);
             CalcularNumeroDeConscuentesPorRegla(pathNumeroConsecuentes, numeroDeConsecuentes);
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <para>Cadena de entrada con delimiador $ por la derecha</para>
        /// <para>Tabla accion</para>
        /// <para>tabla goto</para>
        /// <returns>El Parse, un analisis ascendente de la cadena de entrada. En caso de error una explicación del mismo</returns>
        public string GetParse()
        {
            AnalisisLexico.Token tokenDeEntrada=alex.GetToken();
            string parse="";
            List<string>pila = new List<string>();
            pila.Add("0");
            while (true) // mientras no se acabe el fichero. Que peligro tiene ese while true.
            {
                // El ultimo elemento de la pila deberá ser un numero. Si eso no es así  peta.
                string accion = Accion(Int32.Parse(pila[pila.Count - 1]), tokenDeEntrada.Codigo); // la tabla goto?  Codigo no?
                
                if (accion.Substring(0, 1) == "d")
                {
                    /* DESPLAZAR
                     * 1 Meter tokenDeEntrada en la pila
                     * 2 Meter el estado al que se desplaza  en la pila
                     * 3 Leer sig token.
                     */
                     
                    pila.Add(tokenDeEntrada.Codigo);
                    pila.Add(accion.Substring(1,2));
                    tokenDeEntrada = alex.GetToken();
                }
                else if (accion.Substring(0, 1) == "r")
                {
                    /*Reduccion A->B
                     * 1 Sacar (2*Nº de consecuentes de la regla) de la pila
                     * 2 s' = pila.pop()
                     * 2 meter A en la pila
                     * 3 Obtener Goto[s',A]
                     * Generar el parse correspondiente a la regla
                     */
                     
                   int sacar = 2*reglas[Int32.Parse(accion.Substring(1,2))].Item2;
                   pila.RemoveRange(pila.Count-1-sacar,sacar); //revisar si saca lo esperado.
                    //s' = pila.pop()
                   var estado = pila[pila.Count-1];
                   pila.RemoveAt(pila.Count-1);
                    // Meter A en la pila
                   pila.Add(reglas[Int32.Parse(accion.Substring(1,2))].Item1);
                   //tablaGoto[]
                    
                   

                    
                }
                else if (accion.Substring(0, 1) == "acc")
                {
                    return parse;

                }
                else if (accion == ""){
                    //ignorar.
                }
                else
                {
                    //error
                }
            }

            return "";
        }

     

        public List<string[]> CrearTabla(string path,Dictionary<string,int>Columna)
        {// creamos la estructura tabla. Ya sea la tabla goto o la tabla acción.
            using (var reader = new StreamReader(path))
            {
                List<string[]> tabla = new List<string[]>();
                var firstline = reader.ReadLine()?.Split('#'); // csv separado por hashtags.
                if (firstline != null)
                    for (int i = 0; i < firstline.Length; i++)
                    {
                        Columna.Add(firstline[i], i);
                    }

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Split('#'); // csv separado por hashtags.
                    if (line == null)
                    {
                        // Lanzar error fichero no valido.
                    }
                    tabla.Add(line);
                    //Console.WriteLine(line);
                }
                return tabla;
            }
        }

        public void CalcularNumeroDeConscuentesPorRegla(string path, List<(string,int)> reglas) {
            using (var reader = new StreamReader(path)) {
                int i = 1;
                while (!reader.EndOfStream) {
                    var line = reader.ReadLine()?.Split("->");
                    string[] numeroDeConsecuentes = line[1].Split(" ");
                    reglas[i]=(line[0],numeroDeConsecuentes.Length);
                    i++;         
                }
            }
        }

        /// <summary>
        /// Recibe el estado de la cima de la pila y un token. Te dice si es un desplazamiento reducción aceptación o error.
        /// </summary>
        public string Accion(int estadoPila,string token)
        {
            return tablaAccion[estadoPila + 1][columnaAccion[token]]; // Sumamos 1 por que hay un hueco en la fila 1. De forma que estado 0 estaría en fila 1.
        }


        


    

        public void Aceptar()
        {
            
        }
        public void Desplazar(){

        }

        public void Reducir()
        {
            
        }


        
        

        
        
    }
}