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
namespace org.camunda.bpm.engine.rest.dto
{

	/// 
	/// <summary>
	/// @author nico.rehwaldt
	/// </summary>
	public class ExceptionDto
	{

	  protected internal string type;
	  protected internal string message;

	  public ExceptionDto()
	  {

	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }

	  public virtual string Message
	  {
		  get
		  {
			return message;
		  }
		  set
		  {
			this.message = value;
		  }
	  }

	  public static ExceptionDto fromException(Exception e)
	  {

		ExceptionDto dto = new ExceptionDto();

		dto.type = e.GetType().Name;
		dto.message = e.Message;

		return dto;
	  }

	  public static ExceptionDto fromException(Exception e)
	  {

		ExceptionDto dto = new ExceptionDto();

		dto.type = e.GetType().Name;
		dto.message = e.Message;

		return dto;
	  }




	}

}