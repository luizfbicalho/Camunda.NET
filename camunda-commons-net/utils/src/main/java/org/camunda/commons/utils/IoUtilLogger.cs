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
namespace org.camunda.commons.utils
{

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class IoUtilLogger : UtilsLogger
	{

	  public virtual IoUtilException unableToReadInputStream(IOException cause)
	  {
		return new IoUtilException(exceptionMessage("001", "Unable to read input stream"), cause);
	  }

	  public virtual IoUtilException fileNotFoundException(string filename, Exception cause)
	  {
		return new IoUtilException(exceptionMessage("002", "Unable to find file with path '{}'", filename), cause);
	  }

	  public virtual IoUtilException fileNotFoundException(string filename)
	  {
		return fileNotFoundException(filename, null);
	  }

	  public virtual IoUtilException nullParameter(string parameter)
	  {
		return new IoUtilException(exceptionMessage("003", "Parameter '{}' can not be null", parameter));
	  }

	  public virtual IoUtilException unableToReadFromReader(Exception cause)
	  {
		return new IoUtilException(exceptionMessage("004", "Unable to read from reader"), cause);
	  }

	}

}