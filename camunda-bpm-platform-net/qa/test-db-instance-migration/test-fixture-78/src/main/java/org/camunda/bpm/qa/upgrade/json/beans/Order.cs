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
namespace org.camunda.bpm.qa.upgrade.json.beans
{

	public class Order
	{

	  private long id;
	  private string order;
	  private object dueUntil;
	  private bool active;
	  private IList<RegularCustomer> customers;
	  private OrderDetails orderDetails;
	  private object nullValue;

	  public virtual long Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }
	  public virtual string getOrder()
	  {
		return order;
	  }
	  public virtual void setOrder(string order)
	  {
		this.order = order;
	  }
	  public virtual object DueUntil
	  {
		  get
		  {
			return dueUntil;
		  }
		  set
		  {
			this.dueUntil = value;
		  }
	  }
	  public virtual bool Active
	  {
		  get
		  {
			return active;
		  }
		  set
		  {
			this.active = value;
		  }
	  }
	  public virtual IList<RegularCustomer> Customers
	  {
		  get
		  {
			return customers;
		  }
		  set
		  {
			this.customers = value;
		  }
	  }
	  public virtual OrderDetails OrderDetails
	  {
		  get
		  {
			return orderDetails;
		  }
		  set
		  {
			this.orderDetails = value;
		  }
	  }
	  public virtual object NullValue
	  {
		  get
		  {
			return nullValue;
		  }
		  set
		  {
			this.nullValue = value;
		  }
	  }
	}

}