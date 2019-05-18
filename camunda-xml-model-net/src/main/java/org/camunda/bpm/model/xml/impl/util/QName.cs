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
namespace org.camunda.bpm.model.xml.impl.util
{
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class QName
	{

	  private readonly string qualifier;
	  private readonly string localName;

	  public QName(string localName) : this(null, localName)
	  {
	  }

	  public QName(string qualifier, string localName)
	  {
		this.localName = localName;
		this.qualifier = qualifier;
	  }

	  public virtual string Qualifier
	  {
		  get
		  {
			return qualifier;
		  }
	  }

	  public virtual string LocalName
	  {
		  get
		  {
			return localName;
		  }
	  }

	  public static QName parseQName(string identifier)
	  {
		string qualifier;
		string localName;

		string[] split = identifier.Split(":", 2);
		if (split.Length == 2)
		{
		  qualifier = split[0];
		  localName = split[1];
		}
		else
		{
		  qualifier = null;
		  localName = split[0];
		}

		return new QName(qualifier, localName);
	  }

	  public override string ToString()
	  {
		return combine(qualifier, localName);
	  }

	  public static string combine(string qualifier, string localName)
	  {
		if (string.ReferenceEquals(qualifier, null) || qualifier.Length == 0)
		{
		  return localName;
		}
		else
		{
		  return qualifier + ":" + localName;
		}
	  }

	  public override int GetHashCode()
	  {
		int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(localName, null)) ? 0 : localName.GetHashCode());
		result = prime * result + ((string.ReferenceEquals(qualifier, null)) ? 0 : qualifier.GetHashCode());
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
		QName other = (QName) obj;
		if (string.ReferenceEquals(localName, null))
		{
		  if (!string.ReferenceEquals(other.localName, null))
		  {
			return false;
		  }
		}
		else if (!localName.Equals(other.localName))
		{
		  return false;
		}
		if (string.ReferenceEquals(qualifier, null))
		{
		  if (!string.ReferenceEquals(other.qualifier, null))
		  {
			return false;
		  }
		}
		else if (!qualifier.Equals(other.qualifier))
		{
		  return false;
		}
		return true;
	  }


	}

}