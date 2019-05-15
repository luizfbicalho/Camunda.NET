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
	/// A single lane in a BPMN 2.0 LaneSet, currently only used internally for rendering the
	/// diagram. The PVM doesn't actually use the laneSets/lanes.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class Lane : HasDIBounds
	{

	  protected internal string id;
	  protected internal string name;
	  protected internal IList<string> flowNodeIds;

	  protected internal int x = -1;
	  protected internal int y = -1;
	  protected internal int width = -1;
	  protected internal int height = -1;

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


	  public virtual int X
	  {
		  get
		  {
			return x;
		  }
		  set
		  {
			this.x = value;
		  }
	  }


	  public virtual int Y
	  {
		  get
		  {
			return y;
		  }
		  set
		  {
			this.y = value;
		  }
	  }


	  public virtual int Width
	  {
		  get
		  {
			return width;
		  }
		  set
		  {
			this.width = value;
		  }
	  }


	  public virtual int Height
	  {
		  get
		  {
			return height;
		  }
		  set
		  {
			this.height = value;
		  }
	  }



	  public virtual IList<string> FlowNodeIds
	  {
		  get
		  {
			if (flowNodeIds == null)
			{
			  flowNodeIds = new List<string>();
			}
			return flowNodeIds;
		  }
	  }

	}

}