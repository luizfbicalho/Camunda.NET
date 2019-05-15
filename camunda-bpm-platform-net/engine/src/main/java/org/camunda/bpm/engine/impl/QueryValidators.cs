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
namespace org.camunda.bpm.engine.impl
{
	using Context = org.camunda.bpm.engine.impl.context.Context;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class QueryValidators
	{

	  public class AdhocQueryValidator<T> : Validator<T>
	  {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static final AdhocQueryValidator INSTANCE = new AdhocQueryValidator();
		public static readonly AdhocQueryValidator INSTANCE = new AdhocQueryValidator();

		internal AdhocQueryValidator()
		{
		}

		public virtual void validate(T query)
		{
		  if (!Context.ProcessEngineConfiguration.EnableExpressionsInAdhocQueries && query.Expressions.Count > 0)
		  {
			throw new BadUserRequestException("Expressions are forbidden in adhoc queries. This behavior can be toggled" + " in the process engine configuration");
		  }
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends AbstractQuery<?, ?>> AdhocQueryValidator<T> get()
		public static AdhocQueryValidator<T> get<T>()
		{
		  return (AdhocQueryValidator<T>) INSTANCE;
		}

	  }

	  public class StoredQueryValidator<T> : Validator<T>
	  {

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("rawtypes") public static final StoredQueryValidator INSTANCE = new StoredQueryValidator();
		public static readonly StoredQueryValidator INSTANCE = new StoredQueryValidator();

		internal StoredQueryValidator()
		{
		}

		public virtual void validate(T query)
		{
		  if (!Context.ProcessEngineConfiguration.EnableExpressionsInStoredQueries && query.Expressions.Count > 0)
		  {
			throw new BadUserRequestException("Expressions are forbidden in stored queries. This behavior can be toggled" + " in the process engine configuration");
		  }
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static <T extends AbstractQuery<?, ?>> StoredQueryValidator<T> get()
		public static StoredQueryValidator<T> get<T>()
		{
		  return (StoredQueryValidator<T>) INSTANCE;
		}
	  }

	}

}