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

	using Transactional = org.springframework.transaction.annotation.Transactional;

	/// <summary>
	/// Service bean that handles loan requests.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class LoanRequestBean
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @PersistenceContext private javax.persistence.EntityManager entityManager;
	  private EntityManager entityManager;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Transactional public LoanRequest newLoanRequest(String customerName, System.Nullable<long> amount)
	  public virtual LoanRequest newLoanRequest(string customerName, long? amount)
	  {
		LoanRequest lr = new LoanRequest();
		lr.CustomerName = customerName;
		lr.Amount = amount;
		lr.Approved = false;
		entityManager.persist(lr);
		return lr;
	  }

	  public virtual LoanRequest getLoanRequest(long? id)
	  {
	   return entityManager.find(typeof(LoanRequest), id);
	  }
	}

}