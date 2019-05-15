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
namespace org.camunda.bpm.engine.impl.util.xml
{
	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class Namespace
	{

	  private readonly string namespaceUri;
	  private readonly string alternativeUri;

	  public Namespace(string namespaceUri) : this(namespaceUri, null)
	  {
	  }

	  /// <summary>
	  /// Creates a namespace with an alternative uri.
	  /// </summary>
	  /// <param name="namespaceUri"> </param>
	  /// <param name="alternativeUri"> </param>
	  public Namespace(string namespaceUri, string alternativeUri)
	  {
		this.namespaceUri = namespaceUri;
		this.alternativeUri = alternativeUri;
	  }

	  /// <summary>
	  /// If a namespace has changed over time it could feel responsible for handling
	  /// the older one.
	  /// 
	  /// @return
	  /// </summary>
	  public virtual bool hasAlternativeUri()
	  {
		return !string.ReferenceEquals(alternativeUri, null);
	  }

	  public virtual string NamespaceUri
	  {
		  get
		  {
			return namespaceUri;
		  }
	  }

	  public virtual string AlternativeUri
	  {
		  get
		  {
			return alternativeUri;
		  }
	  }

	  public override int GetHashCode()
	  {
		const int prime = 31;
		int result = 1;
		result = prime * result + ((string.ReferenceEquals(namespaceUri, null)) ? 0 : namespaceUri.GetHashCode());
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
		Namespace other = (Namespace) obj;
		if (string.ReferenceEquals(namespaceUri, null))
		{
		  if (!string.ReferenceEquals(other.namespaceUri, null))
		  {
			return false;
		  }
		}
		else if (!namespaceUri.Equals(other.namespaceUri))
		{
		  return false;
		}
		return true;
	  }

	}

}