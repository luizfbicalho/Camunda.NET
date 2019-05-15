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
namespace org.camunda.bpm.qa.upgrade.json.beans
{

	public class OrderDetails
	{

	  private string article;
	  private double price;
	  private int roundedPrice;
	  private IList<string> currencies;
	  private bool paid;

	  public virtual string Article
	  {
		  get
		  {
			return article;
		  }
		  set
		  {
			this.article = value;
		  }
	  }
	  public virtual double Price
	  {
		  get
		  {
			return price;
		  }
		  set
		  {
			this.price = value;
		  }
	  }
	  public virtual int RoundedPrice
	  {
		  get
		  {
			return roundedPrice;
		  }
		  set
		  {
			this.roundedPrice = value;
		  }
	  }
	  public virtual IList<string> Currencies
	  {
		  get
		  {
			return currencies;
		  }
		  set
		  {
			this.currencies = value;
		  }
	  }
	  public virtual bool Paid
	  {
		  get
		  {
			return paid;
		  }
		  set
		  {
			this.paid = value;
		  }
	  }
	}

}