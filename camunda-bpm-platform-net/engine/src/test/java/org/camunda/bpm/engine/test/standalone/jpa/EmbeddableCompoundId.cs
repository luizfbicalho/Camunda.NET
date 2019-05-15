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
namespace org.camunda.bpm.engine.test.standalone.jpa
{

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Embeddable @SuppressWarnings("serial") public class EmbeddableCompoundId implements java.io.Serializable
	[Serializable]
	public class EmbeddableCompoundId
	{

	  private long idPart1;

	  private string idPart2;

	  public virtual long IdPart1
	  {
		  get
		  {
			return idPart1;
		  }
		  set
		  {
			this.idPart1 = value;
		  }
	  }


	  public virtual string IdPart2
	  {
		  get
		  {
			return idPart2;
		  }
		  set
		  {
			this.idPart2 = value;
		  }
	  }


//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: @Override public boolean equals(final Object obj)
	  public override bool Equals(object obj)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final EmbeddableCompoundId other = (EmbeddableCompoundId) obj;
		EmbeddableCompoundId other = (EmbeddableCompoundId) obj;
		return idPart1 == other.idPart1 && idPart2.Equals(idPart2);
	  }

	  public override int GetHashCode()
	  {
		return (idPart1 + idPart2).GetHashCode();
	  }

	}

}