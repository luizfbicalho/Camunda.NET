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
namespace org.camunda.bpm.engine.impl.pvm.process
{


	/// <summary>
	/// A BPMN 2.0 LaneSet, containg <seealso cref="Lane"/>s, currently only used for
	/// rendering the DI info.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class LaneSet
	{

	  protected internal string id;
	  protected internal IList<Lane> lanes;
	  protected internal string name;

	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
		  get
		  {
			return id;
		  }
	  }



	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
		  set
		  {
			this.name = value;
		  }
	  }



	  public virtual IList<Lane> Lanes
	  {
		  get
		  {
			if (lanes == null)
			{
			  lanes = new List<Lane>();
			}
			return lanes;
		  }
	  }

	  public virtual void addLane(Lane laneToAdd)
	  {
		Lanes.Add(laneToAdd);
	  }

	  public virtual Lane getLaneForId(string id)
	  {
		if (lanes != null && lanes.Count > 0)
		{
		  foreach (Lane lane in lanes)
		  {
			if (id.Equals(lane.Id))
			{
			  return lane;
			}
		  }
		}
		return null;
	  }
	}
}