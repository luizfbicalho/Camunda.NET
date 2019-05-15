﻿using System;

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
namespace org.camunda.bpm.engine.cdi.test.impl.beans
{

	using BusinessProcessScoped = org.camunda.bpm.engine.cdi.annotation.BusinessProcessScoped;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named @BusinessProcessScoped public class CreditCard implements java.io.Serializable
	[Serializable]
	public class CreditCard
	{

	  private string creditcardNumber;

	  public virtual string CreditcardNumber
	  {
		  set
		  {
			this.creditcardNumber = value;
		  }
		  get
		  {
			return creditcardNumber;
		  }
	  }


	  public override bool Equals(object obj)
	  {
		if (this == obj)
		{
		  return true;
		}
		if (obj == null)
		{
		  return false;
		}
		if (this.GetType() != obj.GetType())
		{
		  return false;
		}
		CreditCard other = (CreditCard) obj;
		if (string.ReferenceEquals(creditcardNumber, null))
		{
		  if (!string.ReferenceEquals(other.creditcardNumber, null))
		  {
			return false;
		  }
		}
		else if (!creditcardNumber.Equals(other.creditcardNumber))
		{
		  return false;
		}
		return true;
	  }

	}

}