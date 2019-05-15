using System.Text;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.spin.plugin.variables
{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @XmlRootElement public class XmlSerializable
	public class XmlSerializable
	{

	  private string stringProperty;

	  private int intProperty;

	  private bool booleanProperty;

	  public XmlSerializable()
	  {

	  }

	  public XmlSerializable(string stringProperty, int intProperty, bool booleanProperty)
	  {
		this.stringProperty = stringProperty;
		this.intProperty = intProperty;
		this.booleanProperty = booleanProperty;
	  }

	  public virtual string StringProperty
	  {
		  get
		  {
			return stringProperty;
		  }
		  set
		  {
			this.stringProperty = value;
		  }
	  }


	  public virtual int IntProperty
	  {
		  get
		  {
			return intProperty;
		  }
		  set
		  {
			this.intProperty = value;
		  }
	  }


	  public virtual bool BooleanProperty
	  {
		  get
		  {
			return booleanProperty;
		  }
		  set
		  {
			this.booleanProperty = value;
		  }
	  }


	  public virtual string toExpectedXmlString()
	  {
		StringBuilder jsonBuilder = new StringBuilder();

		jsonBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><xmlSerializable><booleanProperty>");
		jsonBuilder.Append(booleanProperty);
		jsonBuilder.Append("</booleanProperty><intProperty>");
		jsonBuilder.Append(intProperty);
		jsonBuilder.Append("</intProperty><stringProperty>");
		jsonBuilder.Append(stringProperty);
		jsonBuilder.Append("</stringProperty></xmlSerializable>");

		return jsonBuilder.ToString();
	  }

	  public override string ToString()
	  {
		return toExpectedXmlString();
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + (booleanProperty ? 1231 : 1237);
		result = prime * result + intProperty;
		result = prime * result + ((string.ReferenceEquals(stringProperty, null)) ? 0 : stringProperty.GetHashCode());
		return result;
	  }

	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		XmlSerializable other = (XmlSerializable) obj;
		if (booleanProperty != other.booleanProperty)
		{
		  return false;
		}
		if (intProperty != other.intProperty)
		{
		  return false;
		}
		if (string.ReferenceEquals(stringProperty, null))
		{
		  if (!string.ReferenceEquals(other.stringProperty, null))
		  {
			return false;
		  }
		}
		else if (!stringProperty.Equals(other.stringProperty))
		{
		  return false;
		}
		return true;
	  }

	}

}