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
namespace org.camunda.bpm.engine.spring.test.jpa
{

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Entity(name = "SPRING_TEST_ORDER") public class LoanRequest
	public class LoanRequest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Id @GeneratedValue(strategy = javax.persistence.GenerationType.IDENTITY) @Column(name = "ID_") private System.Nullable<long> id;
		private long? id;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Column(name = "CUSTOMER_NAME_") private String customerName;
	  private string customerName;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Column(name = "AMOUNT_") private System.Nullable<long> amount;
	  private long? amount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Column(name = "APPROVED_") private boolean approved;
	  private bool approved;

	  public virtual long? Id
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


	  public virtual string CustomerName
	  {
		  get
		  {
			return customerName;
		  }
		  set
		  {
			this.customerName = value;
		  }
	  }


	  public virtual long? Amount
	  {
		  get
		  {
			return amount;
		  }
		  set
		  {
			this.amount = value;
		  }
	  }


	  public virtual bool Approved
	  {
		  get
		  {
			return approved;
		  }
		  set
		  {
			this.approved = value;
		  }
	  }


	}

}