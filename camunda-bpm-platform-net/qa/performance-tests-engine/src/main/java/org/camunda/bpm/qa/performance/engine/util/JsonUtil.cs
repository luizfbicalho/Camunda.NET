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
namespace org.camunda.bpm.qa.performance.engine.util
{

	using PerfTestException = org.camunda.bpm.qa.performance.engine.framework.PerfTestException;
	using ObjectMapper = org.codehaus.jackson.map.ObjectMapper;
	using SerializationConfig = org.codehaus.jackson.map.SerializationConfig;
	using JsonSerialize = org.codehaus.jackson.map.annotate.JsonSerialize;
	using Inclusion = org.codehaus.jackson.map.annotate.JsonSerialize.Inclusion;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class JsonUtil
	{

	  private static ObjectMapper mapper;

	  public static void writeObjectToFile(string filename, object @object)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.codehaus.jackson.map.ObjectMapper mapper = getMapper();
		ObjectMapper mapper = Mapper;

		try
		{

		  File resultFile = new File(filename);
		  if (resultFile.exists())
		  {
			resultFile.delete();
		  }
		  resultFile.createNewFile();

		  mapper.writerWithDefaultPrettyPrinter().writeValue(resultFile, @object);

		}
		catch (Exception e)
		{
		  throw new PerfTestException("Cannot write object to file " + filename, e);

		}

	  }

	  public static T readObjectFromFile<T>(string filename, Type type)
	  {
			  type = typeof(T);

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.codehaus.jackson.map.ObjectMapper mapper = getMapper();
		ObjectMapper mapper = Mapper;

		try
		{
		  return mapper.readValue(new File(filename), type);

		}
		catch (Exception e)
		{
		  throw new PerfTestException("Cannot read object from file " + filename, e);

		}
	  }

	  public static ObjectMapper Mapper
	  {
		  get
		  {
			if (mapper == null)
			{
			  mapper = new ObjectMapper();
			  SerializationConfig config = mapper.SerializationConfig.withSerializationInclusion(JsonSerialize.Inclusion.NON_EMPTY).without(SerializationConfig.Feature.FAIL_ON_EMPTY_BEANS);
			  mapper.SerializationConfig = config;
    
			}
			return mapper;
		  }
	  }
	}

}