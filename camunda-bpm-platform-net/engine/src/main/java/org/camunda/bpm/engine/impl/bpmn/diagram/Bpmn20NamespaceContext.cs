﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.impl.bpmn.diagram
{



	/// <summary>
	/// XML <seealso cref="NamespaceContext"/> containing the namespaces used by BPMN 2.0 XML documents.
	/// 
	/// Can be used in <seealso cref="XPath.setNamespaceContext(NamespaceContext)"/>.
	/// 
	/// @author Falko Menge
	/// </summary>
	public class Bpmn20NamespaceContext : NamespaceContext
	{

	  public const string BPMN = "bpmn";
	  public const string BPMNDI = "bpmndi";
	  public const string OMGDC = "omgdc";
	  public const string OMGDI = "omgdi";

	  /// <summary>
	  /// This is a protected filed so you can extend that context with your own namespaces if necessary
	  /// </summary>
	  protected internal IDictionary<string, string> namespaceUris = new Dictionary<string, string>();

	  public Bpmn20NamespaceContext()
	  {
		namespaceUris[BPMN] = "http://www.omg.org/spec/BPMN/20100524/MODEL";
		namespaceUris[BPMNDI] = "http://www.omg.org/spec/BPMN/20100524/DI";
		namespaceUris[OMGDC] = "http://www.omg.org/spec/DD/20100524/DI";
		namespaceUris[OMGDI] = "http://www.omg.org/spec/DD/20100524/DC";
	  }

	  public virtual string getNamespaceURI(string prefix)
	  {
		return namespaceUris[prefix];
	  }

	  public virtual string getPrefix(string namespaceURI)
	  {
		return getKeyByValue(namespaceUris, namespaceURI);
	  }

	  public virtual IEnumerator<string> getPrefixes(string namespaceURI)
	  {
		return getKeysByValue(namespaceUris, namespaceURI).GetEnumerator();
	  }

	  private static ISet<T> getKeysByValue<T, E>(IDictionary<T, E> map, E value)
	  {
		ISet<T> keys = new HashSet<T>();
		foreach (KeyValuePair<T, E> entry in map.SetOfKeyValuePairs())
		{
		  if (value.Equals(entry.Value))
		  {
			keys.Add(entry.Key);
		  }
		}
		return keys;
	  }

	  private static T getKeyByValue<T, E>(IDictionary<T, E> map, E value)
	  {
		foreach (KeyValuePair<T, E> entry in map.SetOfKeyValuePairs())
		{
		  if (value.Equals(entry.Value))
		  {
			return entry.Key;
		  }
		}
		return default(T);
	  }

	}

}