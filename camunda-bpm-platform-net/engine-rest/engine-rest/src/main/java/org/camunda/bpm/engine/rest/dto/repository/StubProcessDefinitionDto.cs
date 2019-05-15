﻿/*
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
namespace org.camunda.bpm.engine.rest.dto.repository
{
	/// <summary>
	/// Setters are only needed to create stub results.
	/// 
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class StubProcessDefinitionDto : ProcessDefinitionDto
	{

	  public virtual string Id
	  {
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual string Key
	  {
		  set
		  {
			this.key = value;
		  }
	  }
	  public virtual string Category
	  {
		  set
		  {
			this.category = value;
		  }
	  }
	  public virtual string Description
	  {
		  set
		  {
			this.description = value;
		  }
	  }
	  public virtual string Name
	  {
		  set
		  {
			this.name = value;
		  }
	  }
	  public virtual int Version
	  {
		  set
		  {
			this.version = value;
		  }
	  }
	  public virtual string Resource
	  {
		  set
		  {
			this.resource = value;
		  }
	  }
	  public virtual string DeploymentId
	  {
		  set
		  {
			this.deploymentId = value;
		  }
	  }
	  public virtual string Diagram
	  {
		  set
		  {
			this.diagram = value;
		  }
	  }
	  public virtual bool Suspended
	  {
		  set
		  {
			this.suspended = value;
		  }
	  }
	}

}