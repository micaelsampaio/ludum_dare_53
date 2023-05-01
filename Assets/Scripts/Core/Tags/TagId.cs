using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tags
{
  public class TagId : MonoBehaviour
  {
    public string id;
    public static string GenerateShortUuid()
    {
      Guid guid = Guid.NewGuid();
      byte[] bytes = guid.ToByteArray();
      long longUuid = BitConverter.ToInt64(bytes, 0);
      string shortUuid = Convert.ToString(longUuid, 36);
      return shortUuid.Substring(0, 8);
    }
  }

}
