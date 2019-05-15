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
namespace org.camunda.bpm.engine.rest.sub
{
	using Query = org.camunda.bpm.engine.query.Query;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using VariableResponseProvider = org.camunda.bpm.engine.rest.sub.impl.VariableResponseProvider;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;


	/// <summary>
	/// Base class to unify the getResource(boolean deserialized) and
	/// getResourceBinary() methods for several subclasses. (formerly getVariable()
	/// and getBinaryVariable())
	/// 
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public abstract class AbstractResourceProvider<T, U, DTO>
	{

	  protected internal string id;
	  protected internal ProcessEngine engine;

	  public AbstractResourceProvider(string detailId, ProcessEngine engine)
	  {
		this.id = detailId;
		this.engine = engine;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public DTO getResource(@QueryParam(VariableResource_Fields.DESERIALIZE_VALUE_QUERY_PARAM) @DefaultValue("true") boolean deserializeObjectValue)
	  public virtual DTO getResource(bool deserializeObjectValue)
	  {
		U variableInstance = baseQueryForVariable(deserializeObjectValue).singleResult();
		if (variableInstance != default(U))
		{
		  return transformToDto(variableInstance);
		}
		else
		{
		  throw new InvalidRequestException(Response.Status.NOT_FOUND, ResourceNameForErrorMessage + " with Id '" + id + "' does not exist.");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/data") public javax.ws.rs.core.Response getResourceBinary()
	  public virtual Response ResourceBinary
	  {
		  get
		  {
			U queryResult = baseQueryForBinaryVariable().singleResult();
			if (queryResult != default(U))
			{
			  TypedValue variableInstance = transformQueryResultIntoTypedValue(queryResult);
			  return (new VariableResponseProvider()).getResponseForTypedVariable(variableInstance, id);
			}
			else
			{
			  throw new InvalidRequestException(Response.Status.NOT_FOUND, ResourceNameForErrorMessage + " with Id '" + id + "' does not exist.");
			}
		  }
	  }


	  protected internal virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  protected internal virtual ProcessEngine Engine
	  {
		  get
		  {
			return engine;
		  }
	  }

	  /// <summary>
	  /// Create the query we need for fetching the desired result. Setting
	  /// properties in the query like disableCustomObjectDeserialization() or
	  /// disableBinaryFetching() should be done in this method.
	  /// </summary>
	  protected internal abstract Query<T, U> baseQueryForBinaryVariable();

	  /// <summary>
	  /// TODO change comment Create the query we need for fetching the desired
	  /// result. Setting properties in the query like
	  /// disableCustomObjectDeserialization() or disableBinaryFetching() should be
	  /// done in this method.
	  /// </summary>
	  /// <param name="deserializeObjectValue"> </param>
	  protected internal abstract Query<T, U> baseQueryForVariable(bool deserializeObjectValue);

	  protected internal abstract TypedValue transformQueryResultIntoTypedValue(U queryResult);

	  protected internal abstract DTO transformToDto(U queryResult);

	  protected internal abstract string ResourceNameForErrorMessage {get;}

	}

}