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
	/// Stores waypoints of a diagram edge.
	/// 
	/// @author Falko Menge
	/// </summary>
	[Serializable]
	public class DiagramEdge : DiagramElement
	{

	  private const long serialVersionUID = 1L;

	  private IList<DiagramEdgeWaypoint> waypoints;

	  public DiagramEdge()
	  {
	  }

	  public DiagramEdge(string id, IList<DiagramEdgeWaypoint> waypoints) : base(id)
	  {
		this.waypoints = waypoints;
	  }

	  public override bool Node
	  {
		  get
		  {
			return false;
		  }
	  }

	  public override bool Edge
	  {
		  get
		  {
			return true;
		  }
	  }

	  public virtual IList<DiagramEdgeWaypoint> Waypoints
	  {
		  get
		  {
			return waypoints;
		  }
		  set
		  {
			this.waypoints = value;
		  }
	  }


	}

}