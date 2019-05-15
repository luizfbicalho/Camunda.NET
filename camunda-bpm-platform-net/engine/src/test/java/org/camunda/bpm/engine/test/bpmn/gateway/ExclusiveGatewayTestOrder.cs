using System;

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
namespace org.camunda.bpm.engine.test.bpmn.gateway
{


	/// <summary>
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public class ExclusiveGatewayTestOrder
	{

	  private const long serialVersionUID = 1L;

	  private int price;

	  public ExclusiveGatewayTestOrder(int price)
	  {
		this.price = price;
	  }

	  public virtual int Price
	  {
		  set
		  {
			this.price = value;
		  }
		  get
		  {
			return price;
		  }
	  }


	  public virtual bool Basic
	  {
		  get
		  {
			return price <= 100;
		  }
	  }

	  public virtual bool Standard
	  {
		  get
		  {
			return price > 100 && price < 250;
		  }
	  }

	  public virtual bool Gold
	  {
		  get
		  {
			return price >= 250;
		  }
	  }

	}

}