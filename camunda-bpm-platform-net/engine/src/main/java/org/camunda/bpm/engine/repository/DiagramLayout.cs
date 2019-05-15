using System;
using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.repository
{


	/// <summary>
	/// Stores a two-dimensional graph layout.
	/// 
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class DiagramLayout
	{

	  private const long serialVersionUID = 1L;

	  private IDictionary<string, DiagramElement> elements;

	  public DiagramLayout(IDictionary<string, DiagramElement> elements)
	  {
		this.Elements = elements;
	  }

	  public virtual DiagramNode getNode(string id)
	  {
		DiagramElement element = Elements[id];
		if (element is DiagramNode)
		{
		  return (DiagramNode) element;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual DiagramEdge getEdge(string id)
	  {
		DiagramElement element = Elements[id];
		if (element is DiagramEdge)
		{
		  return (DiagramEdge) element;
		}
		else
		{
		  return null;
		}
	  }

	  public virtual IDictionary<string, DiagramElement> Elements
	  {
		  get
		  {
			return elements;
		  }
		  set
		  {
			this.elements = value;
		  }
	  }


	  public virtual IList<DiagramNode> Nodes
	  {
		  get
		  {
			IList<DiagramNode> nodes = new List<DiagramNode>();
			foreach (KeyValuePair<string, DiagramElement> entry in Elements.SetOfKeyValuePairs())
			{
			  DiagramElement element = entry.Value;
			  if (element is DiagramNode)
			  {
				nodes.Add((DiagramNode) element);
			  }
			}
			return nodes;
		  }
	  }

	}

}