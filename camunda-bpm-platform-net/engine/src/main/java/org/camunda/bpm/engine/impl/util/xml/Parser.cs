﻿/*
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
namespace org.camunda.bpm.engine.impl.util.xml
{


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class Parser
	{

	  protected internal static SAXParserFactory defaultSaxParserFactory = SAXParserFactory.newInstance();

	  public static readonly Parser INSTANCE = new Parser();

	  public virtual Parse createParse()
	  {
		return new Parse(this);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected javax.xml.parsers.SAXParser getSaxParser() throws Exception
	  protected internal virtual SAXParser SaxParser
	  {
		  get
		  {
			return SaxParserFactory.newSAXParser();
		  }
	  }

	  protected internal virtual SAXParserFactory SaxParserFactory
	  {
		  get
		  {
			return defaultSaxParserFactory;
		  }
	  }
	}

}